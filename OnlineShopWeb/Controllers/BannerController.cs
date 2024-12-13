using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{
    public class BannerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ListBanners()
        {
            var Banners = db.Banners.ToList();
            Banners = Banners.Where(x => x.IsDeleted == false).ToList();
            return PartialView("_ListBanners", Banners);
        }
    }
}