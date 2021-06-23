using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Interface;
using YoutubeDownloader.Model;
using Microsoft.Extensions.Configuration;

namespace YoutubeDownloader.Business
{
    public class ServiceManager : IServiceManager
    {
        //todo:dependency injection  yapılırken buna da bakılacak
        public string ApiKey;
        public ServiceManager(IConfiguration configuration)
        {
            ApiKey = configuration["ChipperSettings:ApiKey"].ToString();
        }
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
