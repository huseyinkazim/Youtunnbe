using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Youtunnbe.Models;

namespace Youtunnbe.Helper
{

    public class ServiceManager : IServiceManager
    {
        public static readonly string ApiKey = ConfigurationManager.AppSettings["ApiKey"].ToString();
        public const string IpHost = "http://api.ipstack.com/";
        public Task<string> TakeIpInfoAsync(string ip)
        {
            return Task.Run(() =>
            {
                IPModel model;
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        var postAddress = IpHost + $"{ip}?access_key={ApiKey}";
                        var response = client.DownloadString(postAddress);
                        model = JsonConvert.DeserializeObject<IPModel>(response);
                    }
                }
                catch (Exception e)
                {
                    return string.Empty;

                }
                if (!string.IsNullOrEmpty(model.ip))
                    return model.ToString();
                else
                    return string.Empty;

            }
            );
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}