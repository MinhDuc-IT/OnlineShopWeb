using OnlineShopWeb.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Areas.Admin.Controllers
{
    [AuthenticateUser]
    [AuthorizeUser(Roles = "Admin")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}