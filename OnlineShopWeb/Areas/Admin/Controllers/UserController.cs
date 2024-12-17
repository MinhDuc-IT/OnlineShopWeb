using Microsoft.Owin.BuilderProperties;
using OnlineShopWeb.Attributes;
using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;

namespace OnlineShopWeb.Areas.Admin.Controllers
{
    [AuthenticateUser]
    [AuthorizeUser(Roles = "Admin")]
    public class UserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetUserByPage(int crrPage, int pageSize, string searchText = "")
        {
            var query = db.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(u => u.Email.Contains(searchText));
            }

            var totalRecords = query.Count();

            var users = query
                        .OrderBy(b => b.CustomerId)
                        .Skip((crrPage - 1) * pageSize)
                        .Take(pageSize)
                        .ToList()
                        .Select(u => new
                        {
                            u.CustomerId,
                            u.Name,
                            u.Email,
                            u.Phone,
                            u.Address,
                            u.Role,
                            u.Gender,
                            BirthDay = u.BirthDay.HasValue
                                ? u.BirthDay.Value.ToString("dd/MM/yyyy")
                                : null,
                            ImageBase64 = u.Avatar != null ? Convert.ToBase64String(u.Avatar) : null,
                        })
                        .ToList();

            return Json(new
            {
                success = true,
                data = users,
                totalRecords = totalRecords,
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult postAddUser()
        {
            try
            {
                string Name = Request.Form["Name"];
                string Email = Request.Form["Email"];
                string Password = Request.Form["Password"];
                string BirthDay = Request.Form["BirthDay"];
                string Phone = Request.Form["Phone"];
                string Address = Request.Form["Address"];
                string Role = Request.Form["Role"];
                string Gender = Request.Form["Gender"];
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                {
                    return Json(new { success = false, message = "Vui lòng nhập đủ các trường dữ liệu bắt buộc." });
                }

                var checkEmail = db.Users.FirstOrDefault(x => x.Email == Email);
                if (checkEmail != null)
                {
                    return Json(new { success = false, message = "Email đã tồn tại hãy chọn Email khác." });
                }

                DateTime birthDate;
                DateTime.TryParseExact(
                    BirthDay,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out birthDate
                );
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);
                var user = new User
                {
                    Name = Name,
                    Email = Email,
                    Password = hashedPassword,
                    Phone = Phone,
                    Address = Address,
                    Role = Role,
                };

                if (!string.IsNullOrEmpty(Gender))
                {
                    user.Gender = Convert.ToInt32(Gender);
                }

                if (!string.IsNullOrWhiteSpace(BirthDay))
                {
                    user.BirthDay = birthDate;
                }
                else
                {
                    user.BirthDay = null;
                }

                HttpPostedFileBase file = Request.Files["avatar"];
                if (file != null)
                {
                    byte[] fileBytes;
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        fileBytes = binaryReader.ReadBytes(file.ContentLength);
                        user.Avatar = fileBytes;
                    }
                }
                db.Users.Add(user);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult EditUser(int id)
        {
            var user = db.Users.Find(id);
            return View(user);
        }
        [HttpPost]
        public ActionResult postEditUser()
        {
            try
            {
                int customerId = Convert.ToInt32(Request.Form["Id"]);
                var user = db.Users.Find(customerId);
                if (user == null) return Json(new { success = false, message = "User not found" });

                string Name = Request.Form["Name"];
                string BirthDay = Request.Form["BirthDay"];
                string Phone = Request.Form["Phone"];
                string Address = Request.Form["Address"];
                string Gender = Request.Form["Gender"];

                DateTime birthDate;
                bool isValidDate = DateTime.TryParseExact(
                BirthDay,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out birthDate);

                user.Name = Name;
                user.BirthDay = birthDate;
                user.Phone = Phone;
                user.Address = Address;
                if (!string.IsNullOrEmpty(Gender))
                {
                    user.Gender = Convert.ToInt32(Gender);
                }

                HttpPostedFileBase file = Request.Files["avatar"];
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
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult deleteUser(int Id)
        {
            try
            {
                var user = db.Users.Find(Id);
                var SessionUser = Session["User"] as User;
                if (user == null)
                {
                    return Json(new { success = false, message = "Không thể tìm thấy người dùng với id: " + Id });
                }
                else if (user.CustomerId == SessionUser.CustomerId)
                {
                    return Json(new { success = false, message = "Không thể xóa chính mình" });
                }
                else
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                    return Json(new { success = true, });
                }
            }
            catch
            {
                return Json(new { success = false, });
            }
        }
    }
}