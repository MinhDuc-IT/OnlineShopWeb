using OnlineShopWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{
    public class AccountController : Controller
    {
        // GET: Profile
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult profile()
        {
            return View();
        }
        public ActionResult Address()
        {
            return View();
        }
        public ActionResult myOrder()
        {
            return View();
        }
        public ActionResult changePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult postNewPassword()
        {
            string Email = Request.Form["email"];
            string password = Request.Form["password"];
            string newPassord = Request.Form["newPassword"];

            var user = db.Users.FirstOrDefault(u => u.Email == Email && u.Password == password);
            if (user != null)
            {
                user.Password = newPassord;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        public ActionResult logout()
        {
            //handle logout
            return Redirect("/login");
        }
    }
}