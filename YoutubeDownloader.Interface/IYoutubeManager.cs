using YoutubeDownloader.Model;
using System.Collections.Generic;

namespace YoutubeDownloader.Interface
{
    public interface IYoutubeManager
    {
         IEnumerable<VideoInfo> YoutubeMediaUrls(string YoutubeUrl);
    }
}
