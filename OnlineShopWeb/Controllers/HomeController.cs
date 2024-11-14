using OnlineShopWeb.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShopWeb.Models;

namespace OnlineShopWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToList();

            var brands = _context.Brands
                .Select(b => new BrandViewModel
                {
                    BrandId = b.BrandId,
                    BrandName = b.Name,
                    ProductCount = _context.Products.Count(p => p.BrandId == b.BrandId)
                })
                .ToList();

            ViewData["Brands"] = brands; // Chuyển dữ liệu vào ViewData
            return View(products);
        }


        public ActionResult GetProductsByBrand(int brandId)
        {
            var products = _context.Products
                .Where(p => p.BrandId == brandId)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToList();
            return PartialView("_ProductListPartial", products);
        }

        public ActionResult GetProductsByCategory(int categoryId)
        {
            var products = _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToList();
            return PartialView("_ProductListPartial", products);
        }
    }
}