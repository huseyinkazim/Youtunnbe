using Business;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Youtunnbe.Helper;
using Youtunnbe.Models;
using Youtunnbe.Controllers.Base;

namespace Youtunnbe.Controllers
{
    [LogActionFilter]
    public class HomeController : BaseController
    {

        private IYoutubeManager manager;
        public IServiceManager serviceManager;
        // GET: Home
        public HomeController(IYoutubeManager youtubeManager, IServiceManager serviceManager) :base(serviceManager)
        {
            this.manager = youtubeManager;

        }

        public async Task<ActionResult> Index()
        {

            if (TempData["link"] != null)
            {
                ViewBag.Link = "https://www.youtube.com/watch?v=" + TempData["link"];
                TempData["link"] = null;
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string link)
        {
            Sayac.link = link;
            IEnumerable<Entity.VideoInfo> videoInfos;
            try
            {
                videoInfos = manager.YoutubeMediaUrls(link);
                Sayac.Title = videoInfos.FirstOrDefault().Title;
            }
            catch(Exception e)
            {
                SendErrorMailAsync(e,link);
                return RedirectToAction("Index");
            }

            return View(videoInfos);
        }

        public ActionResult test()
        {
            return View();
        }
        [HttpPost]
        public ActionResult test(string link)
        {
            IEnumerable<Entity.VideoInfo> videoInfos;
            try
            {
                videoInfos = manager.YoutubeMediaUrls(link);
            }
            catch
            {
                return RedirectToAction("Index");
            }

            return View(videoInfos);
        }

    }
}