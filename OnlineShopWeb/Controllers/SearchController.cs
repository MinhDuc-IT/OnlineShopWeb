using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(string query)
        {
            ViewBag.Query = query;
            IEnumerable<Product> items = db.Products.Where(x => x.Name.Contains(query)).ToList();

            // Tải danh sách thương hiệu
            var brands = db.Brands
                .Select(b => new BrandViewModel
                {
                    BrandId = b.BrandId,
                    BrandName = b.Name,
                    ProductCount = db.Products.Count(p => p.BrandId == b.BrandId)
                })
                .ToList();

            ViewData["Brands"] = brands;

            return View(items);
        }
    }
}