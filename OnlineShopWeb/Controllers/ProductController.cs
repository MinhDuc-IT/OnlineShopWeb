using OnlineShopWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController()
        {
            _context = new ApplicationDbContext();
        }

        // Action hiển thị chi tiết sản phẩm
        public ActionResult Details(int id)
        {
            // Lấy sản phẩm theo id
            var product = _context.Products
                .Include("Category")  // Gắn thông tin danh mục
                .Include("Brand")     // Gắn thông tin thương hiệu
                .FirstOrDefault(p => p.ProductId == id && !p.IsDeleted);

            if (product == null)
            {
                return HttpNotFound("Sản phẩm không tồn tại.");
            }

            return View(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}