using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IProcess
    {
        string Decrypt(string chiper, string jsPath);
        Dictionary<string, string> UrlToDictionaryParameters(string link,bool isContainsControl);
    }
}
