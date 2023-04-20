using System.Collections.Generic;

namespace YoutubeDownloader.Interface
{
    public interface IProcess
    {
        string Decrypt(string chiper, string jsPath);
		Dictionary<string, string> ParseUrlToDictionary(string link, bool isContainsControl = true); 
		
		
	}
}
