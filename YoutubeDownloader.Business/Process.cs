using YoutubeDownloader.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web;

namespace YoutubeDownloader.Business
{
	//public class Process : IProcess
	//{
	//	public Dictionary<string, string> UrlToDictionaryParameters(string link, bool isContainsControl = true)
	//	{

	//		if (link.Contains("?") && isContainsControl)
	//		{
	//			link = link.Substring(link.IndexOf('?') + 1);
	//		}
	//		var dictionary = new Dictionary<string, string>();

	//		foreach (var paremeter in link.Split('&', '?'))
	//		{

	//			var baslangic = paremeter.IndexOf("=") + 1;


	//			var parameters = paremeter.Split('=');

	//			dictionary.Add(parameters[0], paremeter.Substring(baslangic));
	//		}
	//		return dictionary;
	//	}
 //       public Dictionary<string, string> UtubeUrlToDictionaryParameters(string link, bool isContainsControl = true)
 //       {
 //           try
 //           {
 //               string urlParameter = string.Empty;
 //               var s1 = HttpUtility.UrlDecode(link.Substring(link.IndexOf("url")));
 //               var s2 = link.Substring(0, link.IndexOf("url"));

 //               if (s1.Contains("?") && isContainsControl)
 //               {
 //                   urlParameter = s1.Substring(s1.IndexOf('?') + 1);

 //               }
 //               var dictionary = new Dictionary<string, string>();
 //               dictionary.Add("url", s1.Substring(4, s1.IndexOf(urlParameter) - 4));
 //               foreach (var paremeter in s2.Split('&', '?'))
 //               {
 //                   if (paremeter == string.Empty)
 //                       continue;
 //                   var baslangic = paremeter.IndexOf("=") + 1;


 //                   var parameters = paremeter.Split('=');
 //                   dictionary.Add(parameters[0], HttpUtility.UrlDecode(HttpUtility.UrlDecode(paremeter.Substring(baslangic))));
 //               }
 //               foreach (var paremeter in urlParameter.Split('&', '?'))
 //               {

 //                   var baslangic = paremeter.IndexOf("=") + 1;


 //                   var parameters = paremeter.Split('=');

 //                   dictionary.Add(parameters[0], HttpUtility.UrlDecode(HttpUtility.UrlDecode(paremeter.Substring(baslangic))));
 //               }
 //               return dictionary;
 //           }
 //           catch (Exception ex)
 //           {
 //               throw;

 //           }
 //       }
	//	private string GetUrlResouces(string url)
	//	{
	//		using (var client = new WebClient())
	//		{
	//			client.Encoding = System.Text.Encoding.UTF8;
	//			return client.DownloadString(url);
	//		}
	//	}
	//	private string GetFunctionFromLine(string currentLine)
	//	{
	//		Regex matchFunctionReg = new Regex(@"\w+\.(?<functionID>\w+)\("); //lc.ac(b,c) want the ac part.
	//		Match rgMatch = matchFunctionReg.Match(currentLine);
	//		string matchedFunction = rgMatch.Groups["functionID"].Value;
	//		return matchedFunction; //return 'ac'
	//	}
	//	private string decodeURIComponent(string chiper)
	//	{
	//		return System.Web.HttpUtility.HtmlDecode(chiper);
	//	}
	//	private string encodeURIComponent(string chiper)
	//	{
	//		return System.Web.HttpUtility.HtmlEncode(chiper);
	//	}
	//	public string Decrypt(string chiper, string jsPath)
	//	{
	//		return encodeURIComponent(DecryptEncryptedSignature_v2(decodeURIComponent(chiper), jsPath));
	//	}
	//	private string DecryptEncryptedSignature_v2(string cipher, string jsPath)
	//	{
	//		string jsUrl = string.Format($"https://www.youtube.com{jsPath}");

	//		string js = GetUrlResouces(jsUrl);

	//		var all = js.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
	//		var s = all.FirstOrDefault(i => i.Contains("join(\"\")") && i.Contains("split(\"\")"));

	//		Action<string> spliceOperation = (parameter) =>
	//		{
	//			int value;
	//			if (!Int32.TryParse(parameter, out value))
	//				throw new Exception("Splice işlemi sırasında beklenmedik bir hata oluştu.");

	//			cipher = cipher.Substring(value);

	//		};
	//		Action reverseOperation = () =>
	//		{
	//			char[] charArray = cipher.ToCharArray();
	//			Array.Reverse(charArray);
	//			cipher = new string(charArray);
	//		};

	//		Action<string> swapOperation = (parameter) =>
	//		{
	//			int value;
	//			if (!Int32.TryParse(parameter, out value))
	//				throw new Exception("Swap işlemi sırasında beklenmedik bir hata oluştu.");
	//			var cipherArray = cipher.ToCharArray();
	//			char temp = cipherArray[0];
	//			cipherArray[0] = cipherArray[value % cipher.Length];
	//			cipherArray[value % cipher.Length] = temp;
	//			cipher = new string(cipherArray);

	//		};
	//		string reverse = string.Empty, splice = string.Empty, swap = string.Empty;
	//		string reverseFinder = "reverse", spliceFinder = "splice", swapFinder = "a[0]=a[b";
	//		foreach (var line in s.Split(';'))
	//		{

	//			var functionName = GetFunctionFromLine(line);
	//			var sprit = line.Split(new char[] { '.', '(' });

	//			if (string.IsNullOrEmpty(line) || line.Contains("split") || line.Contains("join"))
	//				continue;

	//			var test = all.Where(i => i.Contains($"{sprit[1]}:function"));
	//			if (string.IsNullOrEmpty(reverse))
	//			{
	//				if (test.Count(i => i.Contains(reverseFinder)) != 0)
	//				{
	//					reverse = sprit[1];
	//					reverseOperation();
	//				}
	//			}
	//			//else
	//			//    throw new Exception("İmza oluşturulurken beklenmedik hata ile karşılaşıldı");
	//			else if (reverse == sprit[1])
	//				reverseOperation();


	//			if (string.IsNullOrEmpty(splice))
	//			{
	//				if (test.Count(i => i.Contains(spliceFinder)) != 0)
	//				{
	//					splice = sprit[1];
	//					var parameter = sprit[2].Split(new char[] { ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

	//					spliceOperation(parameter[1]);
	//				}
	//			}
	//			//else
	//			//    throw new Exception("İmza oluşturulurken beklenmedik hata ile karşılaşıldı");
	//			else if (splice == sprit[1])
	//			{
	//				var parameter = sprit[2].Split(new char[] { ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
	//				spliceOperation(parameter[1]);
	//			}
	//			if (string.IsNullOrEmpty(swap))
	//			{
	//				if (test.Count(i => i.Contains(swapFinder)) != 0)
	//				{
	//					swap = sprit[1];
	//					var parameter = sprit[2].Split(new char[] { ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
	//					swapOperation(parameter[1]);
	//				}
	//				//else
	//				//    throw new Exception("İmza oluşturulurken beklenmedik hata ile karşılaşıldı");
	//			}
	//			else if (swap == sprit[1])
	//			{
	//				var parameter = sprit[2].Split(new char[] { ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
	//				swapOperation(parameter[1]);
	//			}



	//		}



	//		return cipher;
	//	}

	//}

	public class UrlHelper : IProcess
	{
		public Dictionary<string, string> ParseUrlToDictionary(string url, bool containsControl = true)
		{
			var dictionary = new Dictionary<string, string>();

			try
			{
				Uri uri = new Uri(url);
				string query = uri.Query.TrimStart('?');
				string fragment = uri.Fragment.TrimStart('#');

				AddKeyValuePairsToDictionary(dictionary, query);
				if (containsControl)
				{
					AddKeyValuePairsToDictionary(dictionary, fragment);
				}
			}
			catch (UriFormatException ex)
			{
				throw new ArgumentException("Invalid URL format.", nameof(url), ex);
			}

			return dictionary;
		}

		private void AddKeyValuePairsToDictionary(Dictionary<string, string> dictionary, string keyValuePairsString)
		{
			foreach (var queryParam in keyValuePairsString.Split('&'))
			{
				var keyValue = queryParam.Split('=');

				if (keyValue.Length < 2)
				{
					continue; // If the parameter doesn't have a value, skip it
				}

				string key = HttpUtility.UrlDecode(keyValue[0]);
				string value = HttpUtility.UrlDecode(keyValue[1]);

				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, value);
				}
			}
		}


		private string GetUrlResouces(string url)
		{
			using (var client = new WebClient())
			{
				client.Encoding = System.Text.Encoding.UTF8;
				return client.DownloadString(url);
			}
		}
		private string GetFunctionFromLine(string currentLine)
		{
			Regex matchFunctionReg = new Regex(@"\w+\.(?<functionID>\w+)\("); //lc.ac(b,c) want the ac part.
			Match rgMatch = matchFunctionReg.Match(currentLine);
			string matchedFunction = rgMatch.Groups["functionID"].Value;
			return matchedFunction; //return 'ac'
		}
		public Dictionary<string, string> GetDictionaryFromUrl(string url, bool containsControl = true)
		{
			if (url.Contains("?") && containsControl)
			{
				url = url.Substring(url.IndexOf('?') + 1);
			}

			var dictionary = new Dictionary<string, string>();

			foreach (var parameter in url.Split('&', '?'))
			{
				var baslangic = parameter.IndexOf("=") + 1;
				var parameters = parameter.Split('=');
				dictionary.Add(parameters[0], parameter.Substring(baslangic));
			}

			return dictionary;
		}

		public string Decrypt(string cipher, string jsPath)
		{
			return HttpUtility.HtmlEncode(DecryptEncryptedSignature_v2(HttpUtility.HtmlDecode(cipher), jsPath));
		}
		private string DecryptEncryptedSignature_v2(string cipher, string jsPath)
		{
			string jsUrl = string.Format($"https://www.youtube.com{jsPath}");

			// JavaScript dosyasının içeriğini indir
			string js = GetUrlResouces(jsUrl);

			// Tüm satırları diziye ayır
			var all = js.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			// join ve split fonksiyonlarını içeren satırı bul
			var s = all.FirstOrDefault(i => i.Contains("join(\"\")") && i.Contains("split(\"\")"));

			// splice işlemi için kullanılan fonksiyon
			Action<string> spliceOperation = (parameter) =>
			{
				int value;
				if (!Int32.TryParse(parameter, out value))
					throw new Exception("Splice işlemi sırasında beklenmedik bir hata oluştu.");

				cipher = cipher.Substring(value);

			};

			// reverse işlemi için kullanılan fonksiyon
			Action reverseOperation = () =>
			{
				char[] charArray = cipher.ToCharArray();
				Array.Reverse(charArray);
				cipher = new string(charArray);
			};

			// swap işlemi için kullanılan fonksiyon
			Action<string> swapOperation = (parameter) =>
			{
				int value;
				if (!Int32.TryParse(parameter, out value))
					throw new Exception("Swap işlemi sırasında beklenmedik bir hata oluştu.");
				var cipherArray = cipher.ToCharArray();
				char temp = cipherArray[0];
				cipherArray[0] = cipherArray[value % cipher.Length];
				cipherArray[value % cipher.Length] = temp;
				cipher = new string(cipherArray);

			};

			// reverse, splice ve swap işlemleri için kullanılan değişkenler
			string reverse = string.Empty, splice = string.Empty, swap = string.Empty;
			string reverseFinder = "reverse", spliceFinder = "splice", swapFinder = "a[0]=a[b";
			// JavaScript dosyasının her satırını tarayarak, işlem adımlarını gerçekleştir
			foreach (var line in s.Split(';'))
			{
				var functionName = GetFunctionFromLine(line);
				var sprit = line.Split(new char[] { '.', '(' });

				if (string.IsNullOrEmpty(line) || line.Contains("split") || line.Contains("join"))
					continue;

				var test = all.Where(i => i.Contains($"{sprit[1]}:function"));
				if (string.IsNullOrEmpty(reverse))
				{
					if (test.Count(i => i.Contains(reverseFinder)) != 0)
					{
						reverse = sprit[1];
						reverseOperation();
					}
				}
				//else
				//    throw new Exception("İmza oluşturulurken beklenmedik hata ile karşılaşıldı");
				else if (reverse == sprit[1])
					reverseOperation();


				if (string.IsNullOrEmpty(splice))
				{
					if (test.Count(i => i.Contains(spliceFinder)) != 0)
					{
						splice = sprit[1];
						var parameter = sprit[2].Split(new char[] { ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

						spliceOperation(parameter[1]);
					}
				}
				//else
				//    throw new Exception("İmza oluşturulurken beklenmedik hata ile karşılaşıldı");
				else if (splice == sprit[1])
				{
					var parameter = sprit[2].Split(new char[] { ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
					spliceOperation(parameter[1]);
				}
				if (string.IsNullOrEmpty(swap))
				{
					if (test.Count(i => i.Contains(swapFinder)) != 0)
					{
						swap = sprit[1];
						var parameter = sprit[2].Split(new char[] { ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
						swapOperation(parameter[1]);
					}
					//else
					//    throw new Exception("İmza oluşturulurken beklenmedik hata ile karşılaşıldı");
				}
				else if (swap == sprit[1])
				{
					var parameter = sprit[2].Split(new char[] { ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
					swapOperation(parameter[1]);
				}
			}
			return cipher;
		}
	}

}
