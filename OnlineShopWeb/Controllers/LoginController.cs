using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;

namespace OnlineShopWeb.Controllers
{

    public class LoginController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public ActionResult Login(string name, string password)
        {
            var user = db.Users.FirstOrDefault(account => account.Email == name && account.Password == password);

            if (user == null)
            {
                return Json(new { success = false, message = "Sai thông tin đăng nhập" });
            }
            else
            {
                Session["UserRole"] = user.Role;
                var ticket = new FormsAuthenticationTicket(
                    1, // Phiên bản ticket
                    user.Email, // Tên người dùng (email hoặc username)
                    DateTime.Now, // Thời gian tạo ticket
                    DateTime.Now.AddMinutes(30), // Thời gian hết hạn
                    false, // Chỉ sử dụng một lần (persistent cookie)
                    user.Role, // Gắn role vào đây
                    FormsAuthentication.FormsCookiePath // Đường dẫn cookie
                );

                // Mã hóa ticket
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                // Tạo cookie chứa ticket đã mã hóa
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(authCookie);

                if (user.Role == "Admin")
                {
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin/Home") });
                }
                else if (user.Role == "User")
                {
                    return Json(new { success = true, name = user.Name, redirectUrl = Url.Action("Index", "Home") });
                }
                else
                {
                    return Json(new { success = false, message = "Vai trò không hợp lệ" });
                }
            }
        }
        [HttpPost]
        public JsonResult Signup(string name, string email, string password, string phone, string address)
        {

            var user = db.Users.FirstOrDefault(account => account.Email == email);
            if (user == null)
            {
                var newUser = new User
                {
                    Name = name,
                    Email = email,
                    Password = password,
                    Phone = phone,
                    Address = address,
                    Role = "User"
                };
                db.Users.Add(newUser);
                db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, Message = "Email đã tồn tại " });
            }
        }
    }
}