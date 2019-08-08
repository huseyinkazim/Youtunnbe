using Entity;
using HtmlAgilityPack;
using Jurassic.Library;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Business
{
    public class YoutubeManager
    {
        public static string Signature1 = "sig";
        public static string Signature2 = "s";
        public static IEnumerable<VideoInfo> YoutubeMediaUrls(string YoutubeUrl)
        {

            string videoUrl;
            if (YoutubeUrl == null)
                throw new ArgumentNullException("videoUrl");
            bool isYoutubeUrl = TryNormalizeYoutubeUrl(YoutubeUrl, out videoUrl);
            if (!isYoutubeUrl)
            {
                throw new ArgumentException("URL is not a valid youtube URL!");
            }

            var json = LoadJson(videoUrl);



            string videoTitle = GetVideoTitle(json);

            string path = GetVideoBaseJsPath(json);
            if (string.IsNullOrEmpty(path))
                throw new Exception("Beklenmedik bir hata oluştu");

            var splitByUrls = GetStreamMap(json);
            var adaptiveFmtSplitByUrls = GetAdaptiveStreamMap(json);
             splitByUrls.AddRange(adaptiveFmtSplitByUrls);

            return GetDownloadUrls(json, videoTitle: videoTitle, jsPath: path, splitByUrls: splitByUrls.ToArray()).ToList();


        }

        private static string GetVideoTitle(JObject json)
        {
            JToken title = json["args"]["title"];
            var player_response = JObject.Parse(System.Web.HttpUtility.UrlDecode(json["args"]["player_response"].ToString()));
            var videoDetails = JsonConvert.DeserializeObject<VideoDetail>(player_response["videoDetails"].ToString());

            return string.IsNullOrEmpty(videoDetails.title) ? "videoPlayback" : videoDetails.title;
        }
        private static List<string> GetStreamMap(JObject json)
        {
            JToken streamMap = json["args"]["url_encoded_fmt_stream_map"];

            string streamMapString = streamMap == null ? null : streamMap.ToString();

            if (streamMapString == null || streamMapString.Contains("been+removed"))
            {
                throw new Exception("Video is removed or has an age restriction.");
            }

            return streamMapString.Split(',').ToList();
        }
        private static List<string> GetAdaptiveStreamMap(JObject json)
        {
            JToken streamMap = json["args"]["adaptive_fmts"];

            if (streamMap == null)
            {
                streamMap = json["args"]["url_encoded_fmt_stream_map"];
            }
            var result = streamMap.ToString();
            var s = result.Split(',').Where(i => Cache.Defaults.FirstOrDefault(j => j.FormatCode.ToString() == Process.UrlToDictionaryParameters(i)["itag"].ToString()) != null &&
              Cache.Defaults.FirstOrDefault(j => j.FormatCode.ToString() == Process.UrlToDictionaryParameters(i)["itag"].ToString()).AudioBitrate != 0).ToList();
            return s;
        }

        private static IEnumerable<VideoInfo> GetDownloadUrls(JObject json, string videoTitle, string jsPath, string[] splitByUrls)
        {
            List<VideoInfo> liste = new List<VideoInfo>();


            string signature = string.Empty;
            foreach (string s in splitByUrls)
            {
                IDictionary<string, string> queries = Process.UrlToDictionaryParameters(s);
                string url;

                string itag;


                itag = queries["itag"];

                int formatCode;
                if (!Int32.TryParse(itag, out formatCode))
                    throw new Exception("Uygun format bulunamadı");

                var videoInfo = new VideoInfo(formatCode);
                if (videoInfo.AudioBitrate == 0)
                    continue;
                if (queries.ContainsKey(Signature2) || queries.ContainsKey(Signature1))
                {
                    // requiresDecryption = queries.ContainsKey(Signature1) || queries.ContainsKey(Signature2);

                    string encryptSignature = queries.ContainsKey(Signature2) ? queries[Signature2] : queries[Signature1];
                    signature = Process.Decrypt(encryptSignature, jsPath);

                    url = string.Format("{0}&{1}={2}", queries["url"], Signature1, signature);

                    string fallbackHost = queries.ContainsKey("fallback_host") ? "&fallback_host=" + queries["fallback_host"] : String.Empty;

                    url += fallbackHost;
                }

                else
                {
                    url = queries["url"];
                }


                IDictionary<string, string> parameters = Process.UrlToDictionaryParameters(url);

                if (!parameters.ContainsKey("ratebypass"))
                    url += string.Format("&{0}={1}", "ratebypass", "yes");

                if (videoInfo.FormatCode != 0)
                {
                    videoInfo.DownloadUrl = url;
                    videoInfo.Title = videoTitle;

                    // videoInfo.RequiresDecryption = requiresDecryption;
                }
                else
                {
                    videoInfo.Title = videoTitle;
                    videoInfo.DownloadUrl = url;
                }

                yield return videoInfo;
                // liste.Add(videoInfo);


            }
            //return liste;

        }

        private static string GetVideoBaseJsPath(JObject json)
        {
            JToken js = json["assets"]["js"];
            return js == null ? String.Empty : js.ToString();
        }
        private static bool IsVideoUnavailable(string pageSource)
        {
            const string unavailableContainer = "<div id=\"watch-player-unavailable\">";

            return pageSource.Contains(unavailableContainer);
        }
        private static JObject LoadJson(string url)
        {
            int i = 0;
            var doc = new HtmlDocument();
            WebClient client = new WebClient();
            string html = client.DownloadString(url);
            doc.LoadHtml(html);
            var scripts = doc.DocumentNode.SelectNodes("//script");
            var innerText = scripts.FirstOrDefault(j => j.InnerHtml.Replace(" ", string.Empty).Contains("ytplayer.config=")).InnerText.Replace(" ", string.Empty);
            var baslangic = innerText.IndexOf("ytplayer.config=") + 16;
            var bitis = innerText.IndexOf(";ytplayer.load");
            var json = innerText.Substring(baslangic, bitis - baslangic);

            return JObject.Parse(json);

        }
        public static bool TryNormalizeYoutubeUrl(string url, out string normalizedUrl)
        {
            url = url.Trim();

            url = url.Replace("youtu.be/", "youtube.com/watch?v=");
            url = url.Replace("www.youtube", "youtube");
            url = url.Replace("youtube.com/embed/", "youtube.com/watch?v=");

            if (url.Contains("/v/"))
            {
                url = "http://youtube.com" + new Uri(url).AbsolutePath.Replace("/v/", "/watch?v=");
            }

            url = url.Replace("/watch#", "/watch?");

            IDictionary<string, string> query = Process.UrlToDictionaryParameters(url);

            string v;

            if (!query.TryGetValue("v", out v))
            {
                normalizedUrl = null;
                return false;
            }

            normalizedUrl = "http://youtube.com/watch?v=" + v;

            return true;
        }
    }
    public class DownloadManager
    {
        public static VideoInfo ChooseVideo(IEnumerable<VideoInfo> videoInfos)
        {
            int i = 1, index; bool isRight = false;
            foreach (var item in videoInfos)
            {
                Console.WriteLine(i + ":" + item.ToString());
                i++;
            }

            do
            {
                Console.WriteLine("Lütfen seçeneklerden birini seçiniz:");
                var index_text = Console.ReadLine();
                isRight = Int32.TryParse(index_text, out index);
                if (isRight)
                    isRight = index <= i && index > 0;
            } while (!isRight);
            VideoInfo video = videoInfos.ToArray()[index - 1];
            return video;
        }
        public static void DownloadVideo(VideoInfo video)
        {
            Task.Run(() =>
         {
             var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Youtube";
             VideoDownloader videoDownloader;
             if (video.Resolution != 0)
                 videoDownloader = new VideoDownloader(video,
                      Path.Combine(filePath,
                      RemoveIllegalPathCharacters(video.Title) + "_" + video.Resolution + video.VideoExtension));
             else
                 videoDownloader = new VideoDownloader(video,
                     Path.Combine(filePath,
                     RemoveIllegalPathCharacters(video.Title) + video.VideoExtension));

             videoDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage);

             videoDownloader.DownloadLinkAsync();
         });
        }
        private static string RemoveIllegalPathCharacters(string path)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }
    }


}
