using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using YoutubeDownloader.Interface;
using YoutubeDownloader.Web.Core.Controllers.Base;
using YoutubeDownloader.Web.Core.Helper;
using YoutubeDownloader.Web.Core.Models;
using Microsoft.Extensions.Configuration;
using YoutubeDownloader.Model;

namespace YoutubeDownloader.Web.Core.Controllers
{
	[LogActionFilter]
	public class HomeController : BaseController
	{
		private IYoutubeManager _manager;

		public HomeController(IYoutubeManager youtubeManager, IServiceManager serviceManager, IConfiguration configuration) : base(serviceManager, configuration)
		{
			_manager = youtubeManager;
		}

		public IActionResult Index()
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
			IEnumerable<VideoInfo> videoInfos;
			try
			{
				videoInfos = _manager.GetVideoInfos(link);
			}
			catch (Exception e)
			{
				SendErrorMailAsync(e, link);
				return RedirectToAction("Index");
			}

			return View(videoInfos);
		}

	}
}
