using OnlineShopWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity; // Thêm namespace này
using OnlineShopWeb.Models;
using OnlineShopWeb.Data;

namespace OnlineShopWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Product
        public ActionResult Index(int? id)
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToList();

            if (id.HasValue)
            {
                products = products.Where(p => p.ProductId == id).ToList();
            }

            return View(products);
        }

        public ActionResult ProductCategory(int id)
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.CategoryId == id)
                .ToList();

            var category = _context.Categories.Find(id);
            if (category != null)
            {
                ViewBag.CateName = category.Name;
            }

            ViewBag.CateId = id;
            return View(products);
        }

        public ActionResult GetProductsByBrand(int brandId)
        {
            try
            {
                var products = _context.Products
                    .Where(p => p.BrandId == brandId)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .ToList();

                return PartialView("_ProductListPartial", products);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = _context.Products
                    .Where(p => p.CategoryId == categoryId)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .ToList();

                return PartialView("_ProductListPartial", products);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        //public ActionResult Detail(int id = 1) // Mặc định id = 1
        //{
        //    var item = _context.Products.Find(id);

        //    if (item == null) // Trường hợp sản phẩm không tồn tại
        //    {
        //        item = new Product
        //        {
        //            ProductId = id, // Gán id nếu có, ngược lại là 0
        //            Name = "Sample Product",
        //            BrandId = 1, // Giá trị mẫu cho BrandId
        //            Description = "This is a test product for testing purposes.",
        //            Image = "/images/sample-product.jpg", // Đường dẫn mẫu đến hình ảnh
        //            Price = 100, // Giá mẫu
        //            Stock = 10, // Số lượng tồn kho mẫu
        //            CategoryId = 1, // Giá trị mẫu cho CategoryId
        //            IsDeleted = false, // Chưa bị xoá
        //            CostPrice = 50, // Giá vốn mẫu
        //        };

        //    }

        //    return View(item);
        //}

        public ActionResult Detail(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

    }
}