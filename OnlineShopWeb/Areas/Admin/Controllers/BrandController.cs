using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OnlineShopWeb.Attributes;
using OnlineShopWeb.Data;
using OnlineShopWeb.Models;

namespace OnlineShopWeb.Areas.Admin.Controllers
{
    [AuthenticateUser]
    [AuthorizeUser(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetBrands(string searchTerm )
        {
            var branches = db.Brands
                .Where(b => string.IsNullOrEmpty(searchTerm) || b.Name.Contains(searchTerm))
                .Select(b => new { b.BrandId, b.Name, b.Description })
                .ToList();

            return Json(branches, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBrandbyId(int id)
        {
            var branches = db.Brands.FirstOrDefault(b=>b.BrandId == id);
            return Json(branches, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddBrand(string name, string description)
        {
            var branch = new Brand { Name = name, Description = description };
            db.Brands.Add(branch);
            db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult EditBrand(int branchId, string name, string description)
        {
            var branch = db.Brands.FirstOrDefault(b => b.BrandId == branchId);
            if (branch != null)
            {
                branch.Name = name;
                branch.Description = description;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Branch not found" });
        }

        [HttpPost]
        public JsonResult DeleteBrand(int branchId)
        {
            var branch = db.Brands.FirstOrDefault(b => b.BrandId == branchId);
            if (branch != null)
            {
                db.Brands.Remove(branch);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Branch not found" });
        }
    }
}
