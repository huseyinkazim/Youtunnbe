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
    public class YoutubeManager : IYoutubeManager
    {
        public const string Signature1 = "sig";
        public const string Signature2 = "s";
        public Process process = new Process();
        public IEnumerable<VideoInfo> YoutubeMediaUrls(string YoutubeUrl)
        {

            string videoUrl, VideoId;
            if (YoutubeUrl == null)
                throw new ArgumentNullException("videoUrl");
            bool isYoutubeUrl = TryNormalizeYoutubeUrl(YoutubeUrl, out VideoId);
            if (!isYoutubeUrl)
            {
                throw new ArgumentException("URL is not a valid youtube URL!");
            }
            videoUrl = "http://youtube.com/watch?v=" + VideoId;
            var json = LoadJson(videoUrl);

            string path = GetVideoBaseJsPath(json);
            if (string.IsNullOrEmpty(path))
                throw new Exception("Beklenmedik bir hata oluştu");

            var splitByUrls = GetStreamMap(json);
            var adaptiveFmtSplitByUrls = GetAdaptiveStreamMap(json);
            splitByUrls.AddRange(adaptiveFmtSplitByUrls);


            var parameter = new VideoDownloadParameter
            {
                json = json,
                videoTitle = GetVideoTitle(json),
                jsPath = path,
                splitByUrls = splitByUrls.ToArray(),
                youtubeLinkId = VideoId
            };


            return GetDownloadUrls(parameter).ToList();


        }

        private string GetVideoTitle(JObject json)
        {
            JToken title = json["args"]["title"];
            var player_response = JObject.Parse(System.Web.HttpUtility.UrlDecode(json["args"]["player_response"].ToString()));
            var videoDetails = JsonConvert.DeserializeObject<VideoDetail>(player_response["videoDetails"].ToString());

            return string.IsNullOrEmpty(videoDetails.title) ? "videoPlayback" : videoDetails.title;
        }
        private List<string> GetStreamMap(JObject json)
        {
            JToken streamMap = json["args"]["url_encoded_fmt_stream_map"];

            string streamMapString = streamMap == null ? null : streamMap.ToString();

            if (streamMapString == null || streamMapString.Contains("been+removed"))
            {
                throw new Exception("Video is removed or has an age restriction.");
            }

            return streamMapString.Split(',').ToList();
        }
        private List<string> GetAdaptiveStreamMap(JObject json)
        {
            JToken streamMap = json["args"]["adaptive_fmts"];

            if (streamMap == null)
            {
                streamMap = json["args"]["url_encoded_fmt_stream_map"];
            }
            var result = streamMap.ToString();
            var s = result.Split(',').Where(i => Cache.Defaults.FirstOrDefault(j => j.FormatCode.ToString() == process.UrlToDictionaryParameters(i)["itag"].ToString()) != null &&
              Cache.Defaults.FirstOrDefault(j => j.FormatCode.ToString() == process.UrlToDictionaryParameters(i)["itag"].ToString()).AudioBitrate != 0).ToList();
            return s;
        }

        private IEnumerable<VideoInfo> GetDownloadUrls(VideoDownloadParameter model)
        {
            List<VideoInfo> liste = new List<VideoInfo>();


            string signature = string.Empty;
            foreach (string s in model.splitByUrls)
            {
                IDictionary<string, string> queries = process.UrlToDictionaryParameters(s);
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

                    string encryptSignature = queries.ContainsKey(Signature2) ? queries[Signature2] : queries[Signature1];
                    signature = process.Decrypt(encryptSignature, model.jsPath);

                    url = string.Format("{0}&{1}={2}", queries["url"], Signature1, signature);

                    string fallbackHost = queries.ContainsKey("fallback_host") ? "&fallback_host=" + queries["fallback_host"] : String.Empty;

                    url += fallbackHost;
                }

                else
                {
                    url = queries["url"];
                }


                IDictionary<string, string> parameters = process.UrlToDictionaryParameters(url);

                if (!parameters.ContainsKey("ratebypass"))
                    url += string.Format("&{0}={1}", "ratebypass", "yes");

                videoInfo.DownloadUrl = url;
                videoInfo.Title = model.videoTitle;
                videoInfo.YoutubeLinkId = model.youtubeLinkId;

                yield return videoInfo;
                // liste.Add(videoInfo);


            }
            //return liste;

        }

        private string GetVideoBaseJsPath(JObject json)
        {
            JToken js = json["assets"]["js"];
            return js == null ? String.Empty : js.ToString();
        }
        private bool IsVideoUnavailable(string pageSource)
        {
            const string unavailableContainer = "<div id=\"watch-player-unavailable\">";

            return pageSource.Contains(unavailableContainer);
        }
        private JObject LoadJson(string url)
        {
            int i = 0;
            var doc = new HtmlDocument();

            string html = GetUrlResouces(url);
            doc.LoadHtml(html);
            var scripts = doc.DocumentNode.SelectNodes("//script");
            var innerText = scripts.FirstOrDefault(j => j.InnerHtml.Replace(" ", string.Empty).Contains("ytplayer.config=")).InnerText;
            var baslangic = innerText.IndexOf("ytplayer.config = ") + 18;
            var bitis = innerText.IndexOf(";ytplayer.load");
            var json = innerText.Substring(baslangic, bitis - baslangic);

            return JObject.Parse(json);

        }
        private string GetUrlResouces(string url)
        {
            using (var client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                return client.DownloadString(url);
            }
        }
        private bool TryNormalizeYoutubeUrl(string url, out string UrlId)
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

            IDictionary<string, string> query = process.UrlToDictionaryParameters(url);

            string v;

            if (!query.TryGetValue("v", out v))
            {
                UrlId = null;
                return false;
            }
            UrlId = v;

            return true;
        }
    }



}
