using OnlineShopWeb.Attributes;
using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Areas.Admin.Controllers
{
    [AuthorizeUser(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            IEnumerable<Category> categories = db.Categories.ToList();
            return View(categories);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult createCategory()
        {
            try
            {
                string Name = Request.Form["Name"];
                string Description = Request.Form["Description"];

                Category newObj = new Category();
                newObj.Name = Name;
                newObj.Description = Description;

                db.Categories.Add(newObj);
                db.SaveChanges();

                return Json(new { success = true, message = "Thêm mới danh mục thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Edit(int id)
        {
            var category = db.Categories.Find(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }
        [HttpPost]
        public ActionResult editCategory()
        {
            try
            {
                int categoryId = int.Parse(Request.Form["ID"]);
                var category = db.Categories.FirstOrDefault(x => x.CategoryId == categoryId);
                if (category == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy danh mục" }, JsonRequestBehavior.AllowGet);
                }

                // Lấy giá trị từ form
                string Name = Request.Form["Name"];
                string Description = Request.Form["Description"];
                bool IsDeleted = bool.Parse(Request.Form["IsDelete"]);

                // Cập nhật thuộc tính
                category.Name = Name;
                category.Description = Description;
                category.IsDeleted = IsDeleted;

                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, message = "Sửa danh mục thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var item = db.Categories.Find(id);
                item.IsDeleted = true;
                db.SaveChanges();
                return Json(new { success = true, });
            }
            catch
            {
                return Json(new { success = false, });
            }
        }
    }
}