using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Business
{
    public class Process : IProcess
    {
        public Dictionary<string, string> UrlToDictionaryParameters(string link, bool isContainsControl = true)
        {
          
            if (link.Contains("?") && isContainsControl)
            {
                link = link.Substring(link.IndexOf('?') + 1);
            }
            var dictionary = new Dictionary<string, string>();
       
            foreach (var paremeter in link.Split('&', '?'))
            {

                var baslangic = paremeter.IndexOf("=") + 1;


                var parameters = paremeter.Split('=');

                dictionary.Add(parameters[0], paremeter.Substring(baslangic));
            }
            return dictionary;
        }
        public Dictionary<string, string> UtubeUrlToDictionaryParameters(string link, bool isContainsControl = true)
        {
            try
            {
                string urlParameter=string.Empty;
                var s1 = System.Web.HttpUtility.UrlDecode(link.Substring(link.IndexOf("url")));
                var s2 = link.Substring(0, link.IndexOf("url"));

                if (s1.Contains("?") && isContainsControl)
                {
                    urlParameter = s1.Substring(s1.IndexOf('?') + 1);

                }
                var dictionary = new Dictionary<string, string>();
                dictionary.Add("url", s1.Substring(4, s1.IndexOf(urlParameter)-4));
                foreach (var paremeter in s2.Split('&', '?'))
                {
                    if (paremeter == string.Empty)
                        continue;
                    var baslangic = paremeter.IndexOf("=") + 1;


                    var parameters = paremeter.Split('=');
                    dictionary.Add(parameters[0], paremeter.Substring(baslangic));
                }
                foreach (var paremeter in urlParameter.Split('&', '?'))
                {

                    var baslangic = paremeter.IndexOf("=") + 1;


                    var parameters = paremeter.Split('=');

                    dictionary.Add(parameters[0], paremeter.Substring(baslangic));
                }
                return dictionary;
            }
            catch(Exception ex)
            {
                throw new Exception("");

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
        private string decodeURIComponent(string chiper)
        {
            return System.Web.HttpUtility.HtmlDecode(chiper);
        }
        private string encodeURIComponent(string chiper)
        {
            return System.Web.HttpUtility.HtmlEncode(chiper);
        }
        public string Decrypt(string chiper, string jsPath)
        {
            return encodeURIComponent(DecryptEncryptedSignature_v2(decodeURIComponent(chiper), jsPath));
        }
        private string DecryptEncryptedSignature_v2(string cipher, string jsPath)
        {
            string jsUrl = string.Format($"https://www.youtube.com{jsPath}");

            string js = GetUrlResouces(jsUrl);

            var all = js.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var s = all.FirstOrDefault(i => i.Contains("join(\"\")") && i.Contains("split(\"\")"));

            Action<string> spliceOperation = (parameter) =>
            {
                int value;
                if (!Int32.TryParse(parameter, out value))
                    throw new Exception("Splice işlemi sırasında beklenmedik bir hata oluştu.");

                cipher = cipher.Substring(value);

            };
            Action reverseOperation = () =>
            {
                char[] charArray = cipher.ToCharArray();
                Array.Reverse(charArray);
                cipher = new string(charArray);
            };

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
            string reverse = string.Empty, splice = string.Empty, swap = string.Empty;
            string reverseFinder = "reverse", spliceFinder = "splice", swapFinder = "a[0]=a[b";
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
        public static IDictionary<string, string> ParseQueryString(string s)
        {
            // remove anything other than query string from url
            if (s.Contains("?"))
            {
                s = s.Substring(s.IndexOf('?') + 1);
            }

            var dictionary = new Dictionary<string, string>();

            foreach (string vp in Regex.Split(s, "&"))
            {
                string[] strings = Regex.Split(vp, "=");

                string key = strings[0];
                string value = string.Empty;

                if (strings.Length == 2)
                    value = strings[1];
                else if (strings.Length > 2)
                    value = string.Join("=", strings.Skip(1).ToArray());

                dictionary.Add(key, value);
            }

            return dictionary;
        }
    }

}
