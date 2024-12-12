using OnlineShopWeb.Attributes;
using OnlineShopWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [AuthenticateUser]
        public ActionResult Index()
        {
            return View();
        }

    }
}