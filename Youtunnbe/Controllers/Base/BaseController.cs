using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YoutubeDownloader.Model;
using Youtunnbe.Helper;
using Youtunnbe.Models;

namespace Youtunnbe.Controllers.Base
{
    public class BaseController : Controller
    {
        IServiceManager serviceManager;
        public BaseController(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewResult info=new ViewResult();
            if (filterContext?.Result.GetType() == typeof(ViewResult))
             info = (ViewResult)filterContext.Result;
            
            if (info.Model != null)
            {
                var models = (List<VideoInfo>)info.Model;
                //Sayac.RequestedIp = ;
                SendMailAsync(models, serviceManager.TakeIpInfoAsync(Request.UserHostAddress).Result, 1);
            }

            base.OnActionExecuted(filterContext);
        }
        private Task SendMailAsync(List<VideoInfo> models, string requestedIp, int sayi)
        {
            return Task.Run(() =>
             {
                 var email = Cipher.Decrypt(ConfigurationManager.AppSettings["frommailaddress"]);
                 var password = Cipher.Decrypt(ConfigurationManager.AppSettings["frommaillpassword"]);
                 MailMessage ePosta = new MailMessage();
                 ePosta.From = new MailAddress(email);
                 //
                 ePosta.To.Add(Cipher.Decrypt(ConfigurationManager.AppSettings["tomailaddress"]));

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
                     //Sayac.RequestedIp = string.Empty;
                     //Sayac.link = string.Empty;
                 }
             });

        }

        protected Task SendErrorMailAsync(Exception ex,string youtubeLink)
        {
            return Task.Run(() =>
            {
                var email = Cipher.Decrypt(ConfigurationManager.AppSettings["frommailaddress"]);
                var password = Cipher.Decrypt(ConfigurationManager.AppSettings["frommaillpassword"]);
                MailMessage ePosta = new MailMessage();
                ePosta.From = new MailAddress(email);
                //
                ePosta.To.Add(Cipher.Decrypt(ConfigurationManager.AppSettings["tomailaddress"]));

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