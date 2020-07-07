using Entity;
using HtmlAgilityPack;
using Jurassic.Library;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Business
{
    public class YoutubeManager : IYoutubeManager
    {
        public const string Signature1 = "sig";
        public const string Signature2 = "s";
        public const string DefaultUrl = "https://redirector.googlevideo.com/videoplayback?";


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
            JObject json;

            json = LoadJson(videoUrl);

            string jsPath = GetVideoBaseJsPath(json);

            if (string.IsNullOrEmpty(jsPath))
                throw new Exception("JsPath bulunamadı");

            var models = GetVideoDatas(json);

            List<string> splitByUrls = new List<string>();


            for (int i = 0; i < models.Count; i++)
            {
                if (models[i].signatureCipher != null)
                    splitByUrls.Add(models[i].signatureCipher.ToString());
                else
                    splitByUrls.Add(models[i].url.ToString());

            }


            List<VideoInfo> list = new List<VideoInfo>();


            var parameter = new 
            {
                json = json,
                videoTitle = GetVideoTitle(json),
                jsPath = jsPath,
                splitByUrls = splitByUrls.ToArray(),
                youtubeLinkId = VideoId
            };
            //todo: en son burada kaldım
            list = GetDownloadUrls(parameter).ToList();



            return list;


        }

        private string GetVideoTitle(JObject json)
        {
            JToken title = json["args"]["title"];
            var player_response = JObject.Parse(System.Web.HttpUtility.UrlDecode(json["args"]["player_response"].ToString()));
            var videoTitle = player_response["videoDetails"]["title"].ToString();

            return RemoveInvalidChars(string.IsNullOrEmpty(videoTitle) ? "videoPlayback" : videoTitle);
        }
        public string RemoveInvalidChars(string value)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                value = value.Replace(c, '_');
            }
            return value;
        }
        private List<dynamic> GetVideoDatas(JObject json)
        {
            List<dynamic> videoDatas = new List<dynamic>();

            var response = JObject.Parse(json["args"]["player_response"].ToString())["streamingData"];

            var formatToken = response["formats"];
            var adaptiveFormatsToken = response["adaptiveFormats"];

            var formatDynamic = JsonConvert.DeserializeObject<dynamic>(formatToken.ToString());
            var adaptiveFormatsTokenDynamic = JsonConvert.DeserializeObject<dynamic>(adaptiveFormatsToken.ToString());
            for (int i = 0; i < formatDynamic.Count; i++)
                videoDatas.Add(formatDynamic[i]);

            for (int i = 0; i < adaptiveFormatsToken.Count(); i++)
                videoDatas.Add(adaptiveFormatsToken[i].ToObject<dynamic>());


            return videoDatas;

        }
        private IEnumerable<VideoInfo> GetDownloadUrls(dynamic model)
        {
            List<VideoInfo> liste = new List<VideoInfo>();


            string signature = string.Empty;
            foreach (string s in model["splitByUrls"])
            {
                string url = DefaultUrl;
                IDictionary<string, string> queries;
                if (s.IndexOf("url") != -1)
                {
                    queries = process.UtubeUrlToDictionaryParameters(s);
                }
                else
                {
                    queries = process.UrlToDictionaryParameters(s);
                }
                queries.Add("title", HttpUtility.UrlEncode(model.videoTitle));
                if (queries.ContainsKey("url"))
                    url = queries["url"];
                string itag;


                itag = queries["itag"];

                int formatCode;
                if (!Int32.TryParse(itag, out formatCode))
                    throw new Exception("Uygun format bulunamadı");

                var videoInfo = new VideoInfo(formatCode);
                if (videoInfo.AudioBitrate == 0)
                    continue;

                if (s.IndexOf("url") == -1)
                {
                    videoInfo.DownloadUrl = s + $"&title={HttpUtility.UrlEncode(model.videoTitle)}";

                    videoInfo.Title = model.videoTitle;
                    videoInfo.YoutubeLinkId = model.youtubeLinkId;

                    //yield return videoInfo;
                    liste.Add(videoInfo);
                    queries.Clear();
                    continue;
                }
                if (queries.ContainsKey(Signature2) || queries.ContainsKey(Signature1))
                {

                    string encryptSignature = queries.ContainsKey(Signature2) ? queries[Signature2] : queries[Signature1];
                    signature = process.Decrypt(encryptSignature, model.jsPath);
                    if (url.Contains("&"))
                        url = string.Format("{0}&{1}={2}", url, Signature1, signature);
                    else
                        url = string.Format("{0}{1}={2}", url, Signature1, signature);

                    string fallbackHost = queries.ContainsKey("fallback_host") ? "&fallback_host=" + queries["fallback_host"] : String.Empty;

                    url += fallbackHost;
                }
                //queries.Add("title", HttpUtility.UrlEncode(model.videoTitle));
                foreach (var dic in queries.Where(i => i.Key != Signature1 && i.Key != Signature2))
                {
                    url = string.Format("{0}&{1}={2}", url, dic.Key, dic.Value);
                }
                // IDictionary<string, string> parameters = process.UrlToDictionaryParameters(url);

                if (!queries.ContainsKey("ratebypass"))
                    url += string.Format("&{0}={1}", "ratebypass", "yes");

                videoInfo.DownloadUrl = url + $"&title={HttpUtility.UrlEncode(model.videoTitle)}";
                videoInfo.Title = model.videoTitle;
                videoInfo.YoutubeLinkId = model.youtubeLinkId;

                //yield return videoInfo;
                liste.Add(videoInfo);
                queries.Clear();


            }
            return liste;

        }
        private string GetVideoBaseJsPath(JObject json)
        {
            JToken js = json["assets"]["js"];
            return js == null ? String.Empty : js.ToString();
        }
        private JObject LoadJson(string url)
        {
            var doc = new HtmlDocument();

            string html = GetUrlResouces(url);
            doc.LoadHtml(html);

            var scripts = doc.DocumentNode.SelectNodes("//script");
            var innerText = scripts.FirstOrDefault(j => j.InnerHtml.Replace(" ", string.Empty).Contains("ytplayer.config=")).InnerText;
            var baslangic = innerText.IndexOf("ytplayer.config = ") + 18;
            var bitis = innerText.IndexOf(";ytplayer.web_player_context_config") != -1
                ? innerText.IndexOf(";ytplayer.web_player_context_config")
                : innerText.IndexOf(";ytplayer.load");
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
