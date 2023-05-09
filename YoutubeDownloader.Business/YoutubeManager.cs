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
		private const string SignatureKey = "sp";
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
				youtubeLinkId = videoId,
				isSignature = videoDatas[0].signatureCipher != null,
				jsPath = jsPath
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

				Dictionary<string, string> queries = _process.ParseUrlToDictionary(url, model.isSignature);
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
				var signatureUrl = DefaultUrl;
				var signatureValue = string.Empty;

				foreach (var item in url.Split("&"))
				{
					if (item.Contains("s="))
						signature = item.Substring(2, item.Length - 2);
					else if (item.Contains("sp="))
						signatureValue = item.Substring(3, item.Length - 3);
			
				}
				var descrpytSignature = _process.Decrypt(signature, model.jsPath);
				
				foreach (var dic in queries.Where(i => i.Key != Signature1 && i.Key != Signature2))
				{
					signatureUrl = string.Format("{0}&{1}={2}", signatureUrl, dic.Key, dic.Value);
				}
				if (HttpUtility.UrlDecode(signatureUrl).Contains("&"))
					signatureUrl = string.Format("{0}&{1}={2}", signatureUrl, Signature1, descrpytSignature);
				else
					signatureUrl = string.Format("{0}{1}={2}", signatureUrl, Signature1, descrpytSignature);

				if (!queries.ContainsKey("ratebypass"))
					signatureUrl += string.Format("&{0}={1}", "ratebypass", "yes");
				signatureUrl = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(signatureUrl));
				string fallbackHost = queries.ContainsKey("fallback_host") ? "&fallback_host=" + queries["fallback_host"] : String.Empty;

				signatureUrl += fallbackHost;
				signatureUrl += "&title=" + HttpUtility.UrlEncode(model.videoTitle);
				videoInfo.DownloadUrl = signatureUrl;
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

	
}
