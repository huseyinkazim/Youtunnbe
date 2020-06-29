using Entity;
using Entity.Response;
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
            Ytplyer playerModel;

            var tuple = LoadJson(videoUrl);
            json = tuple.Item1;
            playerModel = tuple.Item2;
            //var allStream = MapToList<Format, AdaptiveFormat>(playerModel.args.responseModel.streamingData.formats, playerModel.args.responseModel.streamingData.adaptiveFormats);
            string path = GetVideoBaseJsPath(json);
            //redirector.googlevideo.com
            if (string.IsNullOrEmpty(path))
                throw new Exception("Beklenmedik bir hata oluştu");

            var models = GetStreamMap(json);

            List<string> splitByUrls = new List<string>();
            // var adaptiveFmtSplitByUrls = GetAdaptiveStreamMap(json);
            if (models[0].signatureCipher != null)
            {
                for (int i = 0; i < models.Count; i++)
                    splitByUrls.Add(models[i].signatureCipher.ToString());
            }
            else
            {
                for (int i = 0; i < models.Count; i++)
                    splitByUrls.Add(models[i].url.ToString());
            }

            List<VideoInfo> list = new List<VideoInfo>();
#if false
            #region DownloadV2
            var parameter = new VideoDownloadParameterv2
            {
                jsPath = path,
                videoTitle = GetVideoTitle(json),
                youtubeLinkId = VideoId,
                AdaptiveFormats = allStream
            };
            list = GetDownloadUrls(parameter).ToList();
            #endregion
#endif
#if true
            #region DownloadV2

            var parameter = new VideoDownloadParameter
            {
                json = json,
                videoTitle = GetVideoTitle(json),
                jsPath = path,
                splitByUrls = splitByUrls.ToArray(),
                youtubeLinkId = VideoId
            };
            list = GetDownloadUrls(parameter).ToList();
            #endregion
#endif


            return list;


        }

        private L MapTo<T, L>(T from)
        {
            var fromType = typeof(T);
            var toType = typeof(L);

            var toModel = Activator.CreateInstance(toType);

            foreach (var prop in fromType.GetProperties())
            {
                var obj = prop.GetValue(from);
                toType.GetProperty(prop.Name).SetValue(toModel, obj);
            }

            return (L)toModel;

        }
        private List<L> MapToList<T, L>(List<T> from, List<L> ListModel)
        {
            //var ListModel =(List<L>)Activator.CreateInstance(typeof(List<L>));
            foreach (var item in from)
            {
                ListModel.Add(MapTo<T, L>(item));
            }


            return ListModel;
        }
        private string GetVideoTitle(JObject json)
        {
            JToken title = json["args"]["title"];
            var player_response = JObject.Parse(System.Web.HttpUtility.UrlDecode(json["args"]["player_response"].ToString()));
            var videoDetails = JsonConvert.DeserializeObject<VideoDetail>(player_response["videoDetails"].ToString());

            return RemoveInvalidChars(string.IsNullOrEmpty(videoDetails.title) ? "videoPlayback" : videoDetails.title);
        }
        public string RemoveInvalidChars(string value)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                value = value.Replace(c, '_');
            }
            return value;
        }
        // private Tuple<List<Entity.Format>, List<AdaptiveFormat>> GetStreamMap(JObject json)
        private List<dynamic> GetStreamMap(JObject json)
        {
            #region dynamic
            List<dynamic> adaptiveformatsDynamic = new List<dynamic>();

            var player_response = json["args"]["player_response"].ToString();

            var response = JObject.Parse(player_response);

            var formatToken = response["streamingData"]["formats"];
            var adaptiveFormatsToken = response["streamingData"]["adaptiveFormats"];

            var formatDynamic = JsonConvert.DeserializeObject<dynamic>(formatToken.ToString());
            var adaptiveFormatsTokenDynamic = JsonConvert.DeserializeObject<dynamic>(adaptiveFormatsToken.ToString());
            for (int i = 0; i < formatDynamic.Count; i++)
                adaptiveformatsDynamic.Add(formatDynamic[i]);

            for (int i = 0; i < adaptiveFormatsToken.Count(); i++)
                adaptiveformatsDynamic.Add(adaptiveFormatsToken[i].ToObject<dynamic>());


            return adaptiveformatsDynamic;
            #endregion
            #region model
            //    var formats = formatToken.ToObject<Entity.Format[]>().ToList();
            //List<AdaptiveFormat> adaptiveformats = new List<AdaptiveFormat>();

            //for (int i = 0; i < adaptiveFormatsToken.Count(); i++)
            //    adaptiveformats.Add(adaptiveFormatsToken[i].ToObject<AdaptiveFormat>());

            // return Tuple.Create(formats, adaptiveformats);
            #endregion
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
        private IEnumerable<VideoInfo> GetDownloadUrls(VideoDownloadParameterv2 model)
        {
            //

            List<VideoInfo> liste = new List<VideoInfo>();
            string signature = string.Empty;
            foreach (var item in model.AdaptiveFormats)
            {
                IDictionary<string, string> queries;
                bool isChipper;
                if ("" != null)
                {
                    //queries = process.UrlToDictionaryParameters(item.url);
                    queries = process.UrlToDictionaryParameters("");
                    isChipper = false;
                }
                else
                {
                    queries = process.UrlToDictionaryParameters(item.cipher, false);
                    isChipper = true;
                }
                queries.Add("title", model.videoTitle);
                string url;

                string itag = queries["itag"];
                int formatCode;
                if (!Int32.TryParse(itag, out formatCode))
                    throw new Exception("Uygun format bulunamadı");
                var videoInfo = new VideoInfo(formatCode);
                if (videoInfo.AudioBitrate == 0)
                    continue;

                if (isChipper)//queries.ContainsKey(Signature2) || queries.ContainsKey(Signature1))
                {

                    string encryptSignature = queries.ContainsKey(Signature2) ? queries[Signature2] : queries[Signature1];
                    signature = process.Decrypt(encryptSignature, model.jsPath);
                    if (queries.ContainsKey(Signature2)) queries.Remove(Signature2); else queries.Remove(Signature1);
                    queries.Add(Signature1, signature);
                    var result = string.Join("&", queries.Select(i => i.Key + "=" + i.Value).ToArray());

                    url = DefaultUrl + result;
                    //url = string.Format("{0}&{1}={2}", queries["url"], Signature1, signature);

                    string fallbackHost = queries.ContainsKey("fallback_host") ? "&fallback_host=" + queries["fallback_host"] : String.Empty;

                    url += fallbackHost;
                }
                else
                {
                    var result = string.Join("&", queries.Select(i => i.Key + "=" + i.Value).ToArray());

                    url = DefaultUrl + result;
                }
                IDictionary<string, string> parameters = process.UrlToDictionaryParameters(url);

                if (!parameters.ContainsKey("ratebypass"))
                    url += string.Format("&{0}={1}", "ratebypass", "yes");

                videoInfo.DownloadUrl = url;
                videoInfo.Title = model.videoTitle;
                videoInfo.YoutubeLinkId = model.youtubeLinkId;

                //yield return videoInfo;
                liste.Add(videoInfo);
            }
            return liste.OrderByDescending(i => i.Resolution);
        }
        private IEnumerable<VideoInfo> GetDownloadUrls(VideoDownloadParameter model)
        {
            List<VideoInfo> liste = new List<VideoInfo>();


            string signature = string.Empty;
            foreach (string s in model.splitByUrls)
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
        private bool IsVideoUnavailable(string pageSource)
        {
            const string unavailableContainer = "<div id=\"watch-player-unavailable\">";

            return pageSource.Contains(unavailableContainer);
        }
        private Tuple<JObject, Ytplyer> LoadJson(string url)
        {
            int i = 0;
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

            var model = JsonConvert.DeserializeObject<Ytplyer>(json);
            var decoded = urldecode(model.Args.PlayerResponse);

            model.Args.responseModel = JsonConvert.DeserializeObject<dynamic>(decoded);
            //var s=response.streamingData.formats.FirstOrDefault().cipher;
            return new Tuple<JObject, Ytplyer>(JObject.Parse(json), model);

        }
        private string urldecode(string source) => System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlDecode(source)).Replace(@"\u0026", @"&");




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
