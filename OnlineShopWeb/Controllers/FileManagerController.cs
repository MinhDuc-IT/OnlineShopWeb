using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{
    public class FileManagerController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload ()
        {
            return View();
        }
    }
}