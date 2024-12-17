using Newtonsoft.Json;
using OnlineShopWeb.Data;
using OnlineShopWeb.Helpers;
using OnlineShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
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
            var SessionUser = Session["User"] as User;
            var user = db.Users.FirstOrDefault(x => x.CustomerId == SessionUser.CustomerId);
            return View(user);
        }
        public ActionResult GetUserInfo()
        {
            var sessionUser = Session["User"] as User;
            if (sessionUser == null)
            {
                return Json(new { success = false, message = "Session user not found." }, JsonRequestBehavior.AllowGet);
            }

            var user = db.Users.FirstOrDefault(x => x.CustomerId == sessionUser.CustomerId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found in database." }, JsonRequestBehavior.AllowGet);
            }

            string avatarBase64 = user.Avatar != null
                ? Convert.ToBase64String(user.Avatar)
                : null;

            return Json(new
            {
                success = true,
                userInfo = new
                {
                    Name = user.Name,
                    Avatar = avatarBase64
                }
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateProfile()
        {
            var SessionUser = Session["User"] as User;
            var user = db.Users.FirstOrDefault(x => x.CustomerId == SessionUser.CustomerId);
            string userName = Request.Form["input_Name"];
            if (Request.Form["gender"] != null)
            {
                int gender = int.Parse(Request.Form["gender"]);
                user.Gender = gender;
            }
            string birthDay = Request.Form["birthDay"];

            user.Name = userName;
            DateTime birthDate;

            bool isValidDate = DateTime.TryParseExact(
                birthDay,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out birthDate);
            if (isValidDate)
            {
                user.BirthDay = birthDate;
            }
            HttpPostedFileBase file = Request.Files["urlImage"];
            if (file != null)
            {
                byte[] fileBytes;
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    fileBytes = binaryReader.ReadBytes(file.ContentLength);
                    user.Avatar = fileBytes;
                }
            }
            db.SaveChanges();
            var userUpdate = db.Users.FirstOrDefault(x => x.CustomerId == SessionUser.CustomerId);

            var dataRespon = new
            {
                userUpdate.Name,
                Avatar = userUpdate.Avatar != null ? "data:image/png;base64," + Convert.ToBase64String(userUpdate.Avatar) : null
            };

            return Json(new { success = true, userUpdate = dataRespon });

        }
        public ActionResult Address()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UpdateAddress(string province, string district, string ward, string details)
        {
            var sessionUser = Session["User"] as User;
            if (sessionUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var user = db.Users.FirstOrDefault(x => x.CustomerId == sessionUser.CustomerId);
            if (user != null)
            {
                user.Address = $"{province}, {district}, {ward}, {details}";

                db.SaveChanges();

                user.Orders = null;
                user.UserProducts = null;
                user.Password = null;

                Session["User"] = user;

                return Json(new { success = true, message = "Địa chỉ đã được cập nhật." });
            }

            return Json(new { success = false, message = "Không tìm thấy người dùng." });
        }

        public ActionResult myOrder()
        {
            return View();
        }
        public ActionResult GetOrdersByStatus(string status)
        {
            // Lấy danh sách đơn hàng từ cơ sở dữ liệu dựa trên trạng thái
            ViewBag.Status = status;

            // Trả về View với dữ liệu đơn hàng
            return PartialView("_OrderList");
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

            var user = db.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null)
            {
                return Json(new { success = false, message = "Thông tin người dùng không chính xác" });
            }
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return Json(new { success = false, message = "Thông tin người dùng không chính xác" });
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassord);
            user.Password = hashedPassword;
            db.SaveChanges();
            return Json(new { success = true });
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string email, string password, bool remember)
        {
            var user = db.Users.FirstOrDefault(account => account.Email == email);

            if (user == null)
            {
                return Json(new { success = false, message = "Sai thông tin đăng nhập" });
            }
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {

                return Json(new { success = false, message = "Sai thông tin đăng nhập" });
            }
            user.Password = null;
            user.Email = null;
            user.Avatar = null;


            Session["User"] = user;
            if (remember)
            {
                string userJson = JsonConvert.SerializeObject(user);
                var encryptData = Security.Encrypt(userJson);
                HttpCookie authCookie = new HttpCookie("AuthCookie", encryptData)
                {
                    Expires = DateTime.Now.AddMinutes(30),
                    HttpOnly = true,
                };
                Response.Cookies.Add(authCookie);
            }
            string redirectUrl = Session["CurrentUrl"] as string;

            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = Url.Action("Index", "Home");
            }
            if (user.Role == "Admin")
            {
                return Json(new { success = true, name = user.Name, redirectUrl = Url.Action("Index", "Admin/Home") });
            }
            else if (user.Role == "User")
            {
                return Json(new { success = true, name = user.Name, redirectUrl = redirectUrl });
            }
            else
            {
                return Json(new { success = false, message = "Vai trò không hợp lệ" });
            }
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(string name, string email, string password, string phone, string confirmPassword)
        {
            var user = db.Users.FirstOrDefault(account => account.Email == email);
            if (user == null)
            {
                if (password != confirmPassword)
                {
                    return Json(new { success = false, Message = "Mật khẩu xác thực không chính xác" });
                }
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                var newUser = new User
                {
                    Name = name,
                    Email = email,
                    Password = hashedPassword,
                    Phone = phone,
                    Role = "User"
                };
                db.Users.Add(newUser);
                db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, Message = "Email đã tồn tại" });
            }
        }

        [HttpPost]
        public ActionResult logout()
        {
            Session.Abandon();
            Session.Clear();

            var cookie = new HttpCookie("SessionId")
            {
                Expires = DateTime.Now.AddDays(-1),
                Path = "/"
            };
            Response.Cookies.Add(cookie);

            var authCookie = new HttpCookie("AuthCookie")
            {
                Expires = DateTime.Now.AddDays(-1),
                Path = "/"
            };
            Response.Cookies.Add(authCookie);
            return RedirectToAction("Login", "Account");
        }
    }
}