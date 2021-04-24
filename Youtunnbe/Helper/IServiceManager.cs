using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Youtunnbe.Models;

namespace Youtunnbe.Helper
{
    public interface IServiceManager: IDisposable
    {
        Task<string> TakeIpInfoAsync(string ip);
    }
}
