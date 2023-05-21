using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using YoutubeDownloader.Model;
using YoutubeDownloader.Web.Core.Helper;
using Microsoft.Extensions.Configuration;
using YoutubeDownloader.Interface;

namespace YoutubeDownloader.Web.Core.Controllers.Base
{
    public class BaseController : Controller
    {
        IServiceManager serviceManager;
        private readonly IConfiguration Configuration;

        public BaseController(IServiceManager serviceManager, IConfiguration configuration)
        {
            this.serviceManager = serviceManager;
            Configuration = configuration;
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewResult info = new ViewResult();
            if (filterContext?.Result.GetType() == typeof(ViewResult))
                info = (ViewResult)filterContext.Result;

            if (info.Model != null)
            {
                var models = (List<VideoInfo>)info.Model;
                //Sayac.RequestedIp = ;
                SendMailAsync(models, serviceManager.TakeIpInfoAsync(HttpContext.Connection.RemoteIpAddress.ToString()).Result, 1);
            }

            base.OnActionExecuted(filterContext);
        }
        private Task SendMailAsync(List<VideoInfo> models, string requestedIp, int sayi)
        {
            return Task.Run(() =>
            {
                var email = Cipher.Decrypt(Configuration["ChipperSettings:frommailaddress"]);
                var password = Cipher.Decrypt(Configuration["ChipperSettings:frommaillpassword"]);
                MailMessage ePosta = new MailMessage();
                ePosta.From = new MailAddress(email);
                //
                ePosta.To.Add(Cipher.Decrypt(Configuration["ChipperSettings:tomailaddress"]));

                ////
                //ePosta.Attachments.Add(new Attachment(@"C:\deneme.txt"));
                ////
                ePosta.Subject = $"{models.FirstOrDefault().Title} {DateTime.Now}";
                //
                ePosta.Body = $"Uygulamanız günlük {DateTime.Now} tarihini baz alarak {sayi} adet istek gelmiştir.\n Request ipleri:";

                ePosta.Body += $"\n {requestedIp} link: https://www.youtube.com/watch?v={models.FirstOrDefault().YoutubeLinkId}";

                //
                SmtpClient smtp = new SmtpClient();
                //
                smtp.Credentials = new System.Net.NetworkCredential(email, password);
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                object userState = ePosta;
                bool kontrol = true;
                try
                {
                    smtp.Send(ePosta);
                }
                catch (SmtpException ex)
                {
                    kontrol = false;

                }
                finally
                {
                   
                }
            });

        }

        protected Task SendErrorMailAsync(Exception ex, string youtubeLink)
        {
            return Task.Run(() =>
            {
                var email = Cipher.Decrypt(Configuration["ChipperSettings:frommailaddress"]);
                var password = Cipher.Decrypt(Configuration["ChipperSettings:frommaillpassword"]);
                MailMessage ePosta = new MailMessage();
                ePosta.From = new MailAddress(email);
                //
                ePosta.To.Add(Cipher.Decrypt(Configuration["ChipperSettings:tomailaddress"]));

                ePosta.Body = $"link: {youtubeLink} \n Beklenmedik hata alınmıştır.\n {ex.ToString()}";

                SmtpClient smtp = new SmtpClient();
                //
                smtp.Credentials = new System.Net.NetworkCredential(email, password);
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                object userState = ePosta;
                try
                {
                    smtp.Send(ePosta);
                }
                catch (SmtpException e)
                {
                }

            });
        }
        public void Dispose()
        {
            serviceManager.Dispose();
        }
    }
}
