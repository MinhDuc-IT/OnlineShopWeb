using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace OnlineShopWeb.Controllers
{

    public class LoginController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext = new ApplicationDbContext();

        // GET: Login
        [HttpPost]

        public JsonResult Login(string name, string password)
        {
            var user = applicationDbContext.Users.FirstOrDefault(account => account.Email == name && account.Password == password);
            if (user == null)
            {
                return Json(new { success = false });
            }
            else
            {
                Session["UserId"] = user.CustomerId;
                Session["UserEmail"] = user.Email;
                Session["UserName"] = user.Name;
                Session["UserRole"] = user.Role;
                return Json(new { success = true, userId = user.CustomerId, userEmail = user.Email, name = user.Name, Role = user.Role });
            }
        }

        [HttpPost]

        public JsonResult Signup(string name, string email, string password, string phone, string address)
        {

            var user = applicationDbContext.Users.FirstOrDefault(account => account.Email == email);
            if (user == null)
            {
                var newUser = new User
                {
                    Name = name,
                    Email = email,
                    Password = password,
                    Phone = phone,
                    Address = address,
                    Role = "user"
                };
                applicationDbContext.Users.Add(newUser);
                applicationDbContext.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, Message = "Email đã tồn tại " });
            }



        }

    }

}

