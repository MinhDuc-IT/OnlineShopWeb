using OnlineShopWeb.Attributes;
using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using OnlineShopWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{

    [AuthenticateUser]
    public class CartController : Controller
    {

        private Cart GetCartFromDatabase(ApplicationDbContext db, int customerId)
        {
            var cart = db.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart { CustomerId = customerId };
                db.Carts.Add(cart);
                db.SaveChanges();
            }

            return cart;
        }
 
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();

            int userId = 5;
            var cart = GetCartFromDatabase(db, userId);
            return View(cart.CartItems);
        }

        public ActionResult GetCartItems()
        {
            var db = new ApplicationDbContext();
            int userId = 5; // Hoặc lấy userId từ session hoặc authentication

            var cart = GetCartFromDatabase(db, userId);
            var cartItems = cart.CartItems; // Lấy danh sách các sản phẩm trong giỏ hàng

            // Trả về một partial view chỉ có danh sách sản phẩm
            return PartialView("_CartItemsList", cartItems);
        }


        public ActionResult ShowCount()
        {
            //Cart cart = (Cart)Session["Cart"];
            var db = new ApplicationDbContext();

            int userId = 5;
            var cart = GetCartFromDatabase(db, userId);
            if (cart != null)
            {
                return Json(new { Count = cart.CartItems.Count }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            int userId = 5;
            var code = new { success = false, msg = "", code = -1, count = 0 };
            var db = new ApplicationDbContext();
            var checkProduct = db.Products.FirstOrDefault(x => x.ProductId == id);
            if (checkProduct != null)
            {
                //Cart cart = (Cart)Session["Cart"];
                Cart cart = GetCartFromDatabase(db, userId);
                if (cart == null)
                {
                    cart = new Cart();
                }
                CartItem item = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = checkProduct.ProductId,
                    Quantity = quantity,
                    Price = checkProduct.Price,
                    TotalPrice = checkProduct.Price * quantity
                };
                
                cart.AddToCart(item, quantity);
                //Session["Cart"] = cart;
                //db.Entry(cart).State = EntityState.Modified; // Đánh dấu cart đã bị thay đổi
                db.SaveChanges();
                code = new { success = true, msg = "Add successfully", code = 1, count = cart.CartItems.Count };
            }
            return Json(code);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var code = new { success = false, msg = "", code = -1, count = 0 };
            //Cart cart = (Cart)Session["Cart"];
            var db = new ApplicationDbContext();
            int userId = 5;
            var cart = GetCartFromDatabase(db, userId);
            if (cart != null)
            {
                var checkProduct = cart.CartItems.FirstOrDefault(x => x.ProductId == id);
                if (checkProduct != null)
                {
                    cart.Remove(id, userId);
                    code = new { success = true, msg = "", code = 1, count = cart.CartItems.Count };
                }

            }
            //Session["Cart"] = cart;
            //db.SaveChanges();
            return Json(code);
        }

        [HttpPost]
        public ActionResult IncreaseQuantity(int id)
        {
            //Cart cart = (Cart)Session["Cart"];
            var db = new ApplicationDbContext();
            int userId = 5;
            var cart = GetCartFromDatabase(db, userId);
            var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
                cartItem.TotalPrice = cartItem.Quantity * cartItem.Price;
                //Session["Cart"] = cart;
                db.SaveChanges();
            }
            return Json(new { success = true, newQuantity = cartItem.Quantity, newTotalPrice = cartItem.TotalPrice, cartCount = cart.CartItems.Count });
        }

        [HttpPost]
        public ActionResult DecreaseQuantity(int id)
        {
            //Cart cart = (Cart)Session["Cart"];
            var db = new ApplicationDbContext();
            int userId = 5;
            var cart = GetCartFromDatabase(db, userId);
            var cartItem = db.CartItems.SingleOrDefault(x => x.ProductId == id && x.Cart.CustomerId == userId);
            if (cartItem != null && cartItem.Quantity > 0)
            {
                cartItem.Quantity--;
                if (cartItem.Quantity == 0)
                    db.CartItems.Remove(cartItem);
                else
                    cartItem.TotalPrice = cartItem.Quantity * cartItem.Price;

                //Session["Cart"] = cart;
                db.SaveChanges();
            }
            return Json(new { success = true, newQuantity = cartItem?.Quantity ?? 0, newTotalPrice = cartItem?.TotalPrice ?? 0, cartCount = cart.CartItems.Count });
        }

        [HttpPost]
        public ActionResult DeleteAll()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart != null)
            {
                cart.ClearCart();
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        public ActionResult DeleteMany(List<int> ids)
        {
            var code = new { success = false, msg = "", code = -1, count = 0 };

            //Cart cart = (Cart)Session["Cart"];
            var db = new ApplicationDbContext();
            int userId = 5;
            var cart = GetCartFromDatabase(db, userId);
            if (cart != null && ids != null && ids.Any())
            {
                // Xóa tất cả sản phẩm có trong danh sách ids
                foreach (var id in ids)
                {
                    cart.Remove(id, userId);
                }

                code = new { success = true, msg = "Items were removed successfully", code = 1, count = cart.CartItems.Count };
            }

            //Session["Cart"] = cart;
            db.SaveChanges();
            return Json(code);
        }

        [HttpPost]
        public JsonResult UpdateSelectedItems(List<SelectedItem> items)
        {
            try
            {
                if (items == null || !items.Any())
                {
                    return Json(new { success = false, message = "No items selected." });
                }

                Session["SelectedItems"] = items;
                return Json(new { success = true, message = "Cart updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }


    }
}