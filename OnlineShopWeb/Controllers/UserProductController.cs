using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShopWeb.Data;
using OnlineShopWeb.Models;

namespace OnlineShopWeb.Controllers
{
    public class UserProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UpdateViewNum(int id)
        {
            var user = Session["User"] as User;
            if (user == null)
            {
                return Json(new { success = false });
            }
            int userId = user.CustomerId;
            var userProduct = db.UserProducts.FirstOrDefault(x => x.UserId == userId && x.ProductId == id);
            if (userProduct != null)
            {
                userProduct.ViewNum += userProduct.ViewNum + 1;
                db.SaveChanges();
                return Json(new { success = true });
            }
            var obj = new UserProduct
            {
                UserId = userId,
                ProductId = id,
                ViewNum = 1,
            };
            db.UserProducts.Add(obj);
            db.SaveChanges();
            return Json(new { success = true });
        }
        public ActionResult GetRecommenProducts()
        {
            var userSession = Session["User"] as User;
            if (userSession != null)
            {
                var user = db.Users.Find(userSession.CustomerId);
                var recommendedProducts = user.UserProducts
                                       .Where(up => up.ViewNum > 0)
                                       .OrderByDescending(up => up.ViewNum)
                                       .Select(up => up.Product)
                                       .ToList();
                return PartialView("_RecommenProducts", recommendedProducts);
            }
            return PartialView("_RecommenProducts", null);
        }
    }
}