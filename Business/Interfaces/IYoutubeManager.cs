using Entity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IYoutubeManager
    {
         IEnumerable<VideoInfo> YoutubeMediaUrls(string YoutubeUrl);
        
    }
}
