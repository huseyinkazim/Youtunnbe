using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Interface
{
    public interface IServiceManager : IDisposable
    {
        Task<string> TakeIpInfoAsync(string ip);
    }
}
