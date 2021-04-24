using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Youtunnbe.Controllers
{
    public class WatchController : Controller
    {
       
        public ActionResult Index(string v)
        {
            TempData["link"] = v;
            return RedirectToAction("Index","Home", new { link = v });
        }
    }
}