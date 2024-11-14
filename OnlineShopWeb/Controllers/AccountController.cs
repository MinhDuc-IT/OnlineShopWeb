using OnlineShopWeb.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult profile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UpdateProfile()
        {
            string userName = Request.Form["input_Name"];
            string gender = Request.Form["gender"];
            string birthDay = Request.Form["birthDay"];

            HttpPostedFileBase file = Request.Files["urlImage"]; 

            if (file != null && file.ContentLength > 0)
            {
                string fileType = file.ContentType;
                int fileSize = file.ContentLength;

                if ((fileType == "image/jpeg" || fileType == "image/png") && fileSize <= 1 * 1024 * 1024)
                {
                    byte[] fileBytes;
                    string base64String;
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        fileBytes = binaryReader.ReadBytes(file.ContentLength);
                        base64String = Convert.ToBase64String(fileBytes);
                    }

                    return Json(new { success = true, imageBase64 = base64String, message = "Thông tin và ảnh đã được cập nhật thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Chỉ chấp nhận file ảnh JPEG hoặc PNG với dung lượng tối đa 1 MB." });
                }
            }

            return Json(new { success = true, message = "Thông tin đã được cập nhật nhưng không có ảnh tải lên." });
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