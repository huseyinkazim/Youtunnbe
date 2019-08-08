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
    public class Process
    {
        public static string ReplaceUrlParameterValue(string currentPageUrl, string paramToReplace, string newValue)
        {
            var query = UrlToDictionaryParameters(currentPageUrl);

            query[paramToReplace] = newValue;

            var resultQuery = new StringBuilder();
            bool isFirst = true;

            foreach (KeyValuePair<string, string> pair in query)
            {
                if (!isFirst)
                {
                    resultQuery.Append("&");
                }

                resultQuery.Append(pair.Key);
                resultQuery.Append("=");
                resultQuery.Append(pair.Value);

                isFirst = false;
            }

            var uriBuilder = new UriBuilder(currentPageUrl)
            {
                Query = resultQuery.ToString()
            };

            return uriBuilder.ToString();
        }
        public static Dictionary<string, string> UrlToDictionaryParameters(string link)
        {
           
            if (link.Contains("?"))
            {
                link = link.Substring(link.IndexOf('?') + 1);
            }
            var dictionary = new Dictionary<string, string>();
            foreach (var paremeter in link.Split('&'))
            {

                var baslangic=paremeter.IndexOf("=") + 1;
               

                var parameters = paremeter.Split('=');
               
                    dictionary.Add(parameters[0], System.Web.HttpUtility.UrlDecode(paremeter.Substring(baslangic)));
            }
            return dictionary;
        }
        public static string GetUrlResouces(string url)
        {
            using (var client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                return client.DownloadString(url);
            }
        }
        private static string GetFunctionFromLine(string currentLine)
        {
            Regex matchFunctionReg = new Regex(@"\w+\.(?<functionID>\w+)\("); //lc.ac(b,c) want the ac part.
            Match rgMatch = matchFunctionReg.Match(currentLine);
            string matchedFunction = rgMatch.Groups["functionID"].Value;
            return matchedFunction; //return 'ac'
        }
        public static string decodeURIComponent(string chiper)
        {
            return System.Web.HttpUtility.HtmlDecode(chiper);
        }
        public static string encodeURIComponent(string chiper)
        {
            return System.Web.HttpUtility.HtmlEncode(chiper);
        }
        public static string Decrypt(string chiper, string jsPath)
        {
            return encodeURIComponent(DecryptEncryptedSignature(decodeURIComponent(chiper),  jsPath));
        }
        public static string DecryptEncryptedSignature(string cipher, string jsPath)
        {
            string jsUrl = string.Format($"https://s.ytimg.com{jsPath}");

            string js = GetUrlResouces(jsUrl);


            //Find "C" in this: var A = B.sig||C (B.s)
            string functNamePattern = @"\""signature"",\s?([a-zA-Z0-9\$]+)\(";

            var funcName = Regex.Match(js, functNamePattern).Groups[1].Value;

            if (funcName.Contains("$"))
            {
                funcName = "\\" + funcName; //Due To Dollar Sign Introduction, Need To Escape
            }

            string funcPattern = @"(?!h\.)" + @funcName + @"(\w+)\s*=\s*function\(\s*(\w+)\s*\)\s*{\s*\2\s*=\s*\2\.split\(\""\""\)\s*;(.+)return\s*\2\.join\(\""\""\)\s*}\s*;"; //Escape funcName string
            var funcBody = Regex.Match(js, funcPattern, RegexOptions.Singleline).Value; //Entire sig function
            string line1 = funcBody.Split(new[] { '\r', '\n' }).FirstOrDefault();
            var lines = funcBody.Split(';'); //Each line in sig function

            string idReverse = "", idSlice = "", idCharSwap = ""; //Hold name for each cipher method
            string functionIdentifier = "";
            string operations = "";
            foreach (var line in line1.Split(';')) //Matches the funcBody with each cipher method. Only runs till all three are defined.
            // foreach (var line in lines.Skip(1).Take(lines.Length - 2)) //Matches the funcBody with each cipher method. Only runs till all three are defined.
            {
                if (!string.IsNullOrEmpty(idReverse) && !string.IsNullOrEmpty(idSlice) &&
                    !string.IsNullOrEmpty(idCharSwap))
                {
                    break; //Break loop if all three cipher methods are defined
                }

                functionIdentifier = GetFunctionFromLine(line);
                string reReverse = string.Format(@"{0}:\bfunction\b\(\w+\)", functionIdentifier); //Regex for reverse (one parameter)
                string reSlice = string.Format(@"{0}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\.", functionIdentifier); //Regex for slice (return or not)
                string reSwap = string.Format(@"{0}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b", functionIdentifier); //Regex for the char swap.

                if (Regex.Match(js, reReverse).Success&&string.IsNullOrEmpty(idReverse))
                {
                    idReverse = functionIdentifier; //If def matched the regex for reverse then the current function is a defined as the reverse
                }

                if (Regex.Match(js, reSlice).Success&&string.IsNullOrEmpty(idSlice))
                {
                    idSlice = functionIdentifier; //If def matched the regex for slice then the current function is defined as the slice.
                }

                if (Regex.Match(js, reSwap).Success&&string.IsNullOrEmpty(idCharSwap))
                {
                    idCharSwap = functionIdentifier; //If def matched the regex for charSwap then the current function is defined as swap.
                }
            }
            foreach (var line in line1.Split(';'))

            // foreach (var line in lines.Skip(1).Take(lines.Length - 2))
            {
                Match m;
                functionIdentifier = GetFunctionFromLine(line);

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idCharSwap)
                {
                    operations += "w" + m.Groups["index"].Value + " "; //operation is a swap (w)
                }

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idSlice)
                {
                    operations += "s" + m.Groups["index"].Value + " "; //operation is a slice
                }

                if (functionIdentifier == idReverse) //No regex required for reverse (reverse method has no parameters)
                {
                    operations += "r "; //operation is a reverse
                }
            }

            operations = operations.Trim();

            return DecipherWithOperations(cipher, operations);
        }
        private static string DecipherWithOperations(string cipher, string operations)
        {
            return operations.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(cipher, ApplyOperation);
        }
        private static string ApplyOperation(string cipher, string op)
        {
            switch (op[0])
            {
                case 'r':
                    return new string(cipher.ToCharArray().Reverse().ToArray());

                case 'w':
                    {
                        int index = GetOpIndex(op);
                        return SwapFirstChar(cipher, index);
                    }

                case 's':
                    {
                        int index = GetOpIndex(op);
                        return cipher.Substring(index);
                    }

                default:
                    throw new NotImplementedException("Couldn't find cipher operation.");
            }
        }
        private static int GetOpIndex(string op)
        {
            string parsed = new Regex(@".(\d+)").Match(op).Result("$1");
            int index = Int32.Parse(parsed);

            return index;
        }
        private static string SwapFirstChar(string cipher, int index)
        {
            var builder = new StringBuilder(cipher);
            builder[0] = cipher[index];
            builder[index] = cipher[0];

            return builder.ToString();
        }

    }

}
