using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
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
                .Where(b => string.IsNullOrEmpty(searchTerm) && b.IsDeleted == false || b.Name.Contains(searchTerm)&&b.IsDeleted==false)
                .Select(b => new { b.BrandId, b.Name, b.Description })
                .ToList();

            return Json(branches, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBrandById(int id)
        {
            try
            {
                // Tìm kiếm thương hiệu theo ID và kiểm tra xem nó có bị xóa hay không
                var brand = db.Brands.FirstOrDefault(b => b.BrandId == id && !b.IsDeleted);

                // Kiểm tra nếu không tìm thấy thương hiệu
                if (brand == null)
                {
                    var errorResult = new { success = false, message = "Brand not found or has been deleted" };
                    var jsonError = JsonConvert.SerializeObject(errorResult, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    return Content(jsonError, "application/json");
                }

                // Trả về dữ liệu thương hiệu
                var successResult = new { success = true, data = brand };
                var jsonData = JsonConvert.SerializeObject(successResult, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                return Content(jsonData, "application/json");
            }
            catch (Exception ex)
            {
                var errorResult = new { success = false, message = ex.Message };
                var jsonError = JsonConvert.SerializeObject(errorResult, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                return Content(jsonError, "application/json");
            }
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
            return Json(new { success = false, message = "Brand not found" });
        }

        [HttpPost]
        public JsonResult DeleteBrand(int branchId)
        {
            var branch = db.Brands.FirstOrDefault(b => b.BrandId == branchId);
            if (branch != null)
            {
                 branch.IsDeleted=true;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Branch not found" });
        }
    }
}
