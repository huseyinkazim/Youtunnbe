using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoutubeDownloader.Web.Core.Controllers
{
    public class WatchController : Controller
    {
        public IActionResult Index(string v)
        {
            TempData["link"] = v;
            return RedirectToAction("Index", "Home", new { link = v });
        }
    }
}
