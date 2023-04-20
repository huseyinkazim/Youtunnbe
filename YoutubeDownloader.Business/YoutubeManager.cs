using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using YoutubeDownloader.Interface;
using YoutubeDownloader.Model;

namespace YoutubeDownloader.Business
{
	public class YoutubeManager : IYoutubeManager
	{
		private readonly IProcess _process;
		private const string Signature1 = "sig";
		private const string Signature2 = "s";
		private const string DefaultUrl = "https://redirector.googlevideo.com/videoplayback?";

		public YoutubeManager(IProcess process)
		{

			_process = process ?? throw new ArgumentNullException(nameof(process));
		}

		public IEnumerable<VideoInfo> GetVideoInfos(string youtubeUrl)
		{
			if (youtubeUrl == null)
				throw new ArgumentNullException(nameof(youtubeUrl));

			if (!TryNormalizeYoutubeUrl(youtubeUrl, out string videoId))
				throw new ArgumentException("URL is not a valid youtube URL!", nameof(youtubeUrl));

			var tuple = LoadJson(videoId);
			var json = tuple.Item1;
			var jsPath = tuple.Item2;
			if (json["playabilityStatus"]["status"].ToString() != "OK")
				throw new Exception(json["playabilityStatus"].ToString());

			//var jsPath = GetVideoBaseJsPath(videoId);

			if (string.IsNullOrEmpty(jsPath))
				throw new Exception("JsPath bulunamadı");

			var videoDatas = GetVideoDatas(json);

			var splitByUrls = videoDatas
				.Select(model => model.signatureCipher != null ? model.signatureCipher.ToString() : model.url.ToString())
				.ToList();

			var parameter = new
			{
				videoTitle = GetVideoTitle(json),
				splitByUrls = splitByUrls.ToArray(),
				youtubeLinkId = videoId
			};

			return GetDownloadUrls(parameter);
		}

		private string GetVideoTitle(JObject json)
		{
			var videoTitle = json["videoDetails"]["title"].ToString();
			return RemoveInvalidChars(string.IsNullOrEmpty(videoTitle) ? "videoPlayback" : videoTitle);
		}

		private string RemoveInvalidChars(string value)
		{
			return string.Concat(TurkishChrToEnglishChr(value).Split(Path.GetInvalidFileNameChars()));
		}
		public string TurkishChrToEnglishChr(string text)
		{
			if (string.IsNullOrEmpty(text)) return text;

			Dictionary<char, char> TurkishChToEnglishChDic = new Dictionary<char, char>()
		{
			{'ç','c'},
			{'Ç','C'},
			{'ğ','g'},
			{'Ğ','G'},
			{'ı','i'},
			{'İ','I'},
			{'ş','s'},
			{'Ş','S'},
			{'ö','o'},
			{'Ö','O'},
			{'ü','u'},
			{'Ü','U'}
		};

			return text.Aggregate(new StringBuilder(), (sb, chr) =>
			{
				if (TurkishChToEnglishChDic.ContainsKey(chr))
					sb.Append(TurkishChToEnglishChDic[chr]);
				else
					sb.Append(chr);

				return sb;
			}).ToString();
		}
		private List<dynamic> GetVideoDatas(JObject json)
		{
			var videoDatas = new List<dynamic>();

			var response = json["streamingData"];
			if (response == null)
				throw new Exception("Video bulunamadı");

			var formatDynamic = JsonConvert.DeserializeObject<dynamic>(response["formats"].ToString());
			var adaptiveFormatsTokenDynamic = JsonConvert.DeserializeObject<dynamic>(response["adaptiveFormats"].ToString());
			videoDatas.AddRange(formatDynamic);
			videoDatas.AddRange(adaptiveFormatsTokenDynamic);

			return videoDatas;
		}

		private IEnumerable<VideoInfo> GetDownloadUrls(dynamic model)
		{
			var urls = new List<VideoInfo>();

			foreach (var url in model.splitByUrls)
			{
				//var queries = url.Contains("url")
				//	? _process.UtubeUrlToDictionaryParameters(url)
				//	: _process.UrlToDictionaryParameters(url);
				var queries = _process.ParseUrlToDictionary(url);
				queries.Add("title", HttpUtility.UrlEncode(model.videoTitle));
				var itag = queries["itag"];

				if (!int.TryParse(itag, out int formatCode))
					throw new Exception("Uygun format bulunamadı");

				var videoInfo = new VideoInfo(formatCode);
				if (videoInfo.AudioBitrate == 0)
					continue;

				if (!url.Contains("url"))
				{
					videoInfo.DownloadUrl = url + $"&title={HttpUtility.UrlEncode(model.videoTitle)}";
					videoInfo.Title = model.videoTitle;
					videoInfo.YoutubeLinkId = model.youtubeLinkId;

					urls.Add(videoInfo);
					queries.Clear();
					continue;
				}

				var signature = string.Empty;
				//todo burda bir koşul olacak
				//if()
				videoInfo.DownloadUrl = url;
				videoInfo.Title = model.videoTitle;
				videoInfo.YoutubeLinkId = model.youtubeLinkId;

				urls.Add(videoInfo);
				queries.Clear();
			}

			return urls;
		}

		private bool TryNormalizeYoutubeUrl(string videoUrl, out string videoId)
		{
			videoId = null;
			if (string.IsNullOrEmpty(videoUrl))
			{
				return false;
			}

			videoUrl = videoUrl.Trim();
			if (videoUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
				videoUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
			{
				if (videoUrl.Contains("youtube.com/watch", StringComparison.OrdinalIgnoreCase) ||
					videoUrl.Contains("youtube.com/embed", StringComparison.OrdinalIgnoreCase))
				{
					var uri = new Uri(videoUrl);
					var queryParams = HttpUtility.ParseQueryString(uri.Query);
					if (queryParams.AllKeys.Contains("v", StringComparer.OrdinalIgnoreCase))
					{
						videoId = queryParams["v"];
						return true;
					}
				}
				else if (videoUrl.Contains("youtu.be/", StringComparison.OrdinalIgnoreCase))
				{
					var uri = new Uri(videoUrl);
					string[] parts = uri.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
					if (parts.Length == 1)
					{
						videoId = parts[0];
						return true;
					}
				}
			}

			return false;
		}

		private string GetVideoBaseJsPath(string videoId)
		{
			var jsUrl = "http://youtube.com/watch?v=" + videoId;
			var doc = new HtmlDocument();
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.5");
				HttpResponseMessage response = client.GetAsync(jsUrl).Result;
				if (response.IsSuccessStatusCode)
				{
					string html = response.Content.ReadAsStringAsync().Result;
					doc.LoadHtml(html);

					var scripts = doc.DocumentNode.SelectNodes("//script").Select(i => i.InnerHtml);
					var innerText = scripts.FirstOrDefault(j => j.Replace(" ", string.Empty).Contains("player_ias.vflset"));
					innerText = innerText.Substring(innerText.IndexOf("ytcfg.set"));
					var json = parantezAcKapaBul(innerText);

					JToken js = JObject.Parse(json)["WEB_PLAYER_CONTEXT_CONFIGS"]["WEB_PLAYER_CONTEXT_CONFIG_ID_KEVLAR_WATCH"]["jsUrl"];
					return js == null ? String.Empty : js.ToString();
				}
			}
			return string.Empty;
		}

		private Tuple<JObject, string> LoadJson(string videoId)
		{
			var url = "http://youtube.com/watch?v=" + videoId;
			var doc = new HtmlDocument();

			using (HttpClient client = new HttpClient())
			{
				HttpResponseMessage response = client.GetAsync(url).Result;
				if (response.IsSuccessStatusCode)
				{
					string responseBody = response.Content.ReadAsStringAsync().Result;
					doc.LoadHtml(responseBody);

					var scripts = doc.DocumentNode.SelectNodes("//script").Select(i => i.InnerHtml);
					var innerText = scripts.FirstOrDefault(j => j.Replace(" ", string.Empty).Contains("ytInitialPlayerResponse"));
					var json = "{" + susluParantezAcKapaBul(innerText) + "}";



					innerText = scripts.FirstOrDefault(j => j.Replace(" ", string.Empty).Contains("player_ias.vflset"));
					innerText = innerText.Substring(innerText.IndexOf("ytcfg.set"));
					var jsPathjson = parantezAcKapaBul(innerText);

					JToken js = JObject.Parse(jsPathjson)["WEB_PLAYER_CONTEXT_CONFIGS"]["WEB_PLAYER_CONTEXT_CONFIG_ID_KEVLAR_WATCH"]["jsUrl"];

					var path = js == null ? String.Empty : js.ToString();
					return Tuple.Create(JObject.Parse(json), path);

				}
			}
			return null;
		}
		private string susluParantezAcKapaBul(string longStory)
		{
			List<int> lastList = new List<int>(), firstList = new List<int>();
			var first = longStory.IndexOf("{");
			firstList.Add(first);
			int temp = first;
			while (true)
			{
				temp = longStory.IndexOf("}", temp + 1);

				if (temp == -1)
					break;
				lastList.Add(temp);
			}
			temp = first;
			while (true)
			{
				temp = longStory.IndexOf("{", temp + 1);
				if (temp == -1)
					break;

				firstList.Add(temp);
			}
			int i = firstList.Count / 2, j = 0;
			while (true)
			{

				if (i == 0)
					break;
				if (firstList[i] > lastList[j])
				{
					i--;
				}
				else if (i + 1 < firstList.Count && firstList[i + 1] < lastList[j])
				{
					i++;
				}
				else
				{
					firstList.RemoveAt(i);
					j++;
					i = firstList.Count / 2;
				}
			}
			return longStory.Substring(0, lastList[j]).Substring(firstList[0] + 1);
		}
		private string parantezAcKapaBul(string longStory)
		{
			List<int> lastList = new List<int>(), firstList = new List<int>();
			var first = longStory.IndexOf("(");
			firstList.Add(first);
			int temp = first;
			while (true)
			{
				temp = longStory.IndexOf(")", temp + 1);

				if (temp == -1)
					break;
				lastList.Add(temp);
			}
			temp = first;
			while (true)
			{
				temp = longStory.IndexOf("(", temp + 1);
				if (temp == -1)
					break;

				firstList.Add(temp);
			}
			int i = firstList.Count / 2, j = 0;
			while (true)
			{

				if (i == 0)
					break;
				if (firstList[i] > lastList[j])
				{
					i--;
				}
				else if (i + 1 < firstList.Count && firstList[i + 1] < lastList[j])
				{
					i++;
				}
				else
				{
					firstList.RemoveAt(i);
					j++;
					i = firstList.Count / 2;
				}
			}
			return longStory.Substring(0, lastList[j]).Substring(firstList[0] + 1);
		}
	}

	//public class YoutubeManager : IYoutubeManager
	//{
	//    public const string Signature1 = "sig";
	//    public const string Signature2 = "s";
	//    public const string DefaultUrl = "https://redirector.googlevideo.com/videoplayback?";


	//    public Process process = new Process();
	//    public IEnumerable<VideoInfo> YoutubeMediaUrls(string YoutubeUrl)
	//    {

	//        string VideoId;

	//        if (YoutubeUrl == null)
	//            throw new ArgumentNullException("videoUrl");
	//        bool isYoutubeUrl = TryNormalizeYoutubeUrl(YoutubeUrl, out VideoId);
	//        if (!isYoutubeUrl)
	//        {
	//            throw new ArgumentException("URL is not a valid youtube URL!");
	//        }

	//        JObject json;

	//        json = LoadJson(VideoId);
	//        if (json["playabilityStatus"]["status"].ToString() != "OK")
	//            throw new Exception(json["playabilityStatus"].ToString());
	//        string jsPath = GetVideoBaseJsPath(VideoId);

	//        if (string.IsNullOrEmpty(jsPath))
	//            throw new Exception("JsPath bulunamadı");

	//        var models = GetVideoDatas(json);

	//        List<string> splitByUrls = new List<string>();


	//        for (int i = 0; i < models.Count; i++)
	//        {
	//            if (models[i].signatureCipher != null)
	//                splitByUrls.Add(models[i].signatureCipher.ToString());
	//            else
	//                splitByUrls.Add(models[i].url.ToString());

	//        }


	//        List<VideoInfo> list = new List<VideoInfo>();


	//        var parameter = new
	//        {
	//            json = json,
	//            videoTitle = GetVideoTitle(json),
	//            jsPath = jsPath,
	//            splitByUrls = splitByUrls.ToArray(),
	//            youtubeLinkId = VideoId
	//        };
	//        //todo: en son burada kaldım
	//        list = GetDownloadUrls(parameter).ToList();



	//        return list;


	//    }

	//    private string GetVideoTitle(JObject json)
	//    {
	//        var videoTitle = json["videoDetails"]["title"].ToString();

	//        return RemoveInvalidChars(string.IsNullOrEmpty(videoTitle) ? "videoPlayback" : videoTitle);
	//    }
	//    public string RemoveInvalidChars(string value)
	//    {
	//        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
	//        {
	//            value = value.Replace(c, '_');
	//        }
	//        return value;
	//    }
	//    private List<dynamic> GetVideoDatas(JObject json)
	//    {
	//        List<dynamic> videoDatas = new List<dynamic>();

	//        var response = json["streamingData"];
	//        if (response == null)
	//            throw new Exception("Video bulunamadı");
	//        var formatToken = response["formats"];
	//        var adaptiveFormatsToken = response["adaptiveFormats"];

	//        var formatDynamic = JsonConvert.DeserializeObject<dynamic>(formatToken.ToString());
	//        var adaptiveFormatsTokenDynamic = JsonConvert.DeserializeObject<dynamic>(adaptiveFormatsToken.ToString());
	//        for (int i = 0; i < formatDynamic.Count; i++)
	//            videoDatas.Add(formatDynamic[i]);

	//        for (int i = 0; i < adaptiveFormatsToken.Count(); i++)
	//            videoDatas.Add(adaptiveFormatsToken[i].ToObject<dynamic>());


	//        return videoDatas;

	//    }
	//    private IEnumerable<VideoInfo> GetDownloadUrls(dynamic model)
	//    {
	//        List<VideoInfo> liste = new List<VideoInfo>();


	//        string signature = string.Empty;
	//        foreach (string s in model.splitByUrls)
	//        {
	//            string url = DefaultUrl;
	//            IDictionary<string, string> queries;
	//            if (s.IndexOf("url") != -1)
	//            {
	//                queries = process.UtubeUrlToDictionaryParameters(s);
	//            }
	//            else
	//            {
	//                queries = process.UrlToDictionaryParameters(s);
	//            }
	//            queries.Add("title", HttpUtility.UrlEncode(model.videoTitle));
	//            if (queries.ContainsKey("url"))
	//                url = queries["url"];
	//            string itag;


	//            itag = queries["itag"];

	//            int formatCode;
	//            if (!Int32.TryParse(itag, out formatCode))
	//                throw new Exception("Uygun format bulunamadı");

	//            var videoInfo = new VideoInfo(formatCode);
	//            if (videoInfo.AudioBitrate == 0)
	//                continue;

	//            if (s.IndexOf("url") == -1)
	//            {
	//                videoInfo.DownloadUrl = s + $"&title={HttpUtility.UrlEncode(model.videoTitle)}";

	//                videoInfo.Title = model.videoTitle;
	//                videoInfo.YoutubeLinkId = model.youtubeLinkId;

	//                //yield return videoInfo;
	//                liste.Add(videoInfo);
	//                queries.Clear();
	//                continue;
	//            }
	//            if (queries.ContainsKey(Signature2) || queries.ContainsKey(Signature1))
	//            {

	//                string encryptSignature = queries.ContainsKey(Signature2) ? queries[Signature2] : queries[Signature1];
	//                signature = process.Decrypt(encryptSignature, model.jsPath);
	//                if (url.Contains("&"))
	//                    url = string.Format("{0}&{1}={2}", url, Signature1, signature);
	//                else
	//                    url = string.Format("{0}{1}={2}", url, Signature1, signature);

	//                string fallbackHost = queries.ContainsKey("fallback_host") ? "&fallback_host=" + queries["fallback_host"] : String.Empty;

	//                url += fallbackHost;
	//            }
	//            //queries.Add("title", HttpUtility.UrlEncode(model.videoTitle));
	//            foreach (var dic in queries.Where(i => i.Key != Signature1 && i.Key != Signature2))
	//            {
	//                url = string.Format("{0}&{1}={2}", url, dic.Key, dic.Value);
	//            }
	//            // IDictionary<string, string> parameters = process.UrlToDictionaryParameters(url);

	//            if (!queries.ContainsKey("ratebypass"))
	//                url += string.Format("&{0}={1}", "ratebypass", "yes");
	//            url = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(url));

	//            videoInfo.DownloadUrl = url + $"&title={HttpUtility.UrlEncode(model.videoTitle)}";
	//            videoInfo.Title = model.videoTitle;
	//            videoInfo.YoutubeLinkId = model.youtubeLinkId;

	//            //yield return videoInfo;
	//            liste.Add(videoInfo);
	//            queries.Clear();


	//        }
	//        return liste;

	//    }
	//    private string parantezackapabul(string longStory)
	//    {
	//        List<int> lastList = new List<int>(), firstList = new List<int>();
	//        var first = longStory.IndexOf("(");
	//        firstList.Add(first);
	//        int temp = first;
	//        while (true)
	//        {
	//            temp = longStory.IndexOf(")", temp + 1);

	//            if (temp == -1)
	//                break;
	//            lastList.Add(temp);
	//        }
	//        temp = first;
	//        while (true)
	//        {
	//            temp = longStory.IndexOf("(", temp + 1);
	//            if (temp == -1)
	//                break;

	//            firstList.Add(temp);
	//        }
	//        int i = firstList.Count / 2, j = 0;
	//        while (true)
	//        {

	//            if (i == 0)
	//                break;
	//            if (firstList[i] > lastList[j])
	//            {
	//                i--;
	//            }
	//            else if (i + 1 < firstList.Count && firstList[i + 1] < lastList[j])
	//            {
	//                i++;
	//            }
	//            else
	//            {
	//                firstList.RemoveAt(i);
	//                j++;
	//                i = firstList.Count / 2;
	//            }
	//        }
	//        return longStory.Substring(0, lastList[j]).Substring(firstList[0] + 1);
	//    }
	//    private string susluparantezackapabul(string longStory)
	//    {
	//        List<int> lastList = new List<int>(), firstList = new List<int>();
	//        var first = longStory.IndexOf("{");
	//        firstList.Add(first);
	//        int temp = first;
	//        while (true)
	//        {
	//            temp = longStory.IndexOf("}", temp + 1);

	//            if (temp == -1)
	//                break;
	//            lastList.Add(temp);
	//        }
	//        temp = first;
	//        while (true)
	//        {
	//            temp = longStory.IndexOf("{", temp + 1);
	//            if (temp == -1)
	//                break;

	//            firstList.Add(temp);
	//        }
	//        int i = firstList.Count / 2, j = 0;
	//        while (true)
	//        {

	//            if (i == 0)
	//                break;
	//            if (firstList[i] > lastList[j])
	//            {
	//                i--;
	//            }
	//            else if (i + 1 < firstList.Count && firstList[i + 1] < lastList[j])
	//            {
	//                i++;
	//            }
	//            else
	//            {
	//                firstList.RemoveAt(i);
	//                j++;
	//                i = firstList.Count / 2;
	//            }
	//        }
	//        return longStory.Substring(0, lastList[j]).Substring(firstList[0] + 1);
	//    }
	//    private string GetVideoBaseJsPath(string VideoId)
	//    {
	//        var url = "http://youtube.com/watch?v=" + VideoId;

	//        var doc = new HtmlDocument();
	//        string html = GetUrlResouces(url);

	//        doc.LoadHtml(html);

	//        var scripts = doc.DocumentNode.SelectNodes("//script").Select(i => i.InnerHtml);
	//        var innerText = scripts.FirstOrDefault(j => j.Replace(" ", string.Empty).Contains("player_ias.vflset"));
	//        innerText = innerText.Substring(innerText.IndexOf("ytcfg.set"));
	//        var json = parantezackapabul(innerText);

	//        JToken js = JObject.Parse(json)["WEB_PLAYER_CONTEXT_CONFIGS"]["WEB_PLAYER_CONTEXT_CONFIG_ID_KEVLAR_WATCH"]["jsUrl"];
	//        return js == null ? String.Empty : js.ToString();
	//    }
	//    private string GetYoutubeDecodedUrl(string url)
	//    {
	//        return HttpUtility.UrlDecode(HttpUtility.UrlDecode(Regex.Unescape(url)));

	//    }
	//    private JObject LoadJson(string VideoId)
	//    {
	//        string url;
	//        url = $"https://www.youtube.com/get_video_info?video_id={VideoId}&eurl=https://youtube.googleapis.com/v/{VideoId}";
	//        url = $"https://www.youtube.com/get_video_info?html5=1&video_id={VideoId}&cpn=Iw6bUR1Ue4pNPQkp&eurl&ps=desktop-polymer&el=adunit&hl=tr_TR&aqi=e5yEYNvfJ8a8rQG21bCQCQ&sts=18739&lact=3071&cbr=Chrome&cbrver=90.0.4430.85&c=WEB&cver=2.20210422.04.00&cplayer=UNIPLAYER&cos=Windows&cosver=10.0&cplatform=DESKTOP&adformat=15_2_1&break_type=2&encoded_ad_playback_context=CA8QAhgBKgs4cXEwemxVT0twTUIWZTV5RVlOdmZKOGE4clFHMjFiQ1FDUWACdSPOfT-AAcjiAYoDKDABOAVKEwibiffl95fwAhVGXisKHbYqDJJSCRACGMDQ8QJIAmgBcCyQA_6G_6PUDQ%253D%253D&iv_load_policy=1&autoplay=1&width=853&height=480&content_v=8qq0zlUOKpM&authuser=0&living_room_app_mode=LIVING_ROOM_APP_MODE_UNSPECIFIED";
	//        url = "http://youtube.com/watch?v=" + VideoId;

	//        var doc = new HtmlDocument();

	//        string html = GetUrlResouces(url);
	//        //var dic = Process.ParseQueryString(html);
	//        //if (dic.ContainsKey("player_response"))
	//        //    return JObject.Parse(GetYoutubeDecodedUrl(dic["player_response"]));
	//        //else
	//        {
	//            // throw new Exception("");
	//            url = "http://youtube.com/watch?v=" + VideoId;


	//            doc.LoadHtml(html);

	//            var scripts = doc.DocumentNode.SelectNodes("//script");
	//            //var innerText = scripts.FirstOrDefault(j => j.InnerHtml.Replace(" ", string.Empty).Contains("ytplayer.config=")).InnerText;
	//            var innerText = scripts.FirstOrDefault(j => j.InnerHtml.Replace(" ", string.Empty).Contains("ytInitialPlayerResponse")).InnerText;
	//            var json = "{" + susluparantezackapabul(innerText) + "}";

	//            //var baslangic = innerText.IndexOf("ytplayer.config = ") + 18;
	//            //var bitis = innerText.IndexOf(";ytplayer.web_player_context_config") != -1
	//            //    ? innerText.IndexOf(";ytplayer.web_player_context_config")
	//            //    : innerText.IndexOf(";ytplayer.load");
	//            //var json = innerText.Substring(baslangic, bitis - baslangic);

	//            return JObject.Parse(json);
	//        }
	//    }
	//    private string GetUrlResouces(string url)
	//    {
	//        using (var client = new WebClient())
	//        {
	//            client.Encoding = System.Text.Encoding.UTF8;
	//            return client.DownloadString(url);
	//        }
	//    }
	//    private bool TryNormalizeYoutubeUrl(string url, out string UrlId)
	//    {
	//        url = url.Trim();

	//        url = url.Replace("youtu.be/", "youtube.com/watch?v=");
	//        url = url.Replace("www.youtube", "youtube");
	//        url = url.Replace("youtube.com/embed/", "youtube.com/watch?v=");

	//        if (url.Contains("/v/"))
	//        {
	//            url = "http://youtube.com" + new Uri(url).AbsolutePath.Replace("/v/", "/watch?v=");
	//        }

	//        url = url.Replace("/watch#", "/watch?");

	//        IDictionary<string, string> query = process.UrlToDictionaryParameters(url);

	//        string v;

	//        if (!query.TryGetValue("v", out v))
	//        {
	//            UrlId = null;
	//            return false;
	//        }
	//        UrlId = v;

	//        return true;
	//    }
	//}
}
