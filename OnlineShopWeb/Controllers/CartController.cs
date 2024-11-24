using OnlineShopWeb.Attributes;
using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{
    
    public class CartController : Controller
    {
        [AuthorizeUser(Roles = "User")]
        public ActionResult Index()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart == null)
            {
                cart = new Cart
                {
                    CartItems = new List<CartItem>
                    {
                        new CartItem
                        {
                            ProductId = 1, Quantity = 2, Price = 100, TotalPrice = 200,
                            Product = new Product { Name = "Sản phẩm 1", Image = "/images/cart/one.png" }
                        },
                        new CartItem
                        {
                            ProductId = 2, Quantity = 1, Price = 200, TotalPrice = 200,
                            Product = new Product { Name = "Sản phẩm 2", Image = "/images/cart/two.png" }
                        },
                        new CartItem
                        {
                            ProductId = 3, Quantity = 3, Price = 150, TotalPrice = 450,
                            Product = new Product { Name = "Sản phẩm 3", Image = "/images/cart/three.png" }
                        }
                    }
                };
                Session["Cart"] = cart;
            }
            return View(cart.CartItems);
        }

        //public ActionResult ShowCount()
        //{
        //    Cart cart = (Cart)Session["Cart"];
        //    if (cart != null)
        //    {
        //        return Json(new { Count = cart.CartItems.Count }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            var code = new { success = false, msg = "", code = -1, count = 0 };
            var db = new ApplicationDbContext();
            var checkProduct = db.Products.FirstOrDefault(x => x.ProductId == id);
            if (checkProduct != null)
            {
                Cart cart = (Cart)Session["Cart"];
                if (cart == null)
                {
                    cart = new Cart();
                }
                CartItem item = new CartItem
                {
                    ProductId = checkProduct.ProductId,
                    Quantity = quantity
                };
                item.Price = checkProduct.Price;
                item.TotalPrice = item.Quantity * item.Price;
                cart.AddToCart(item, quantity);
                Session["Cart"] = cart;
                code = new { success = true, msg = "Add successfully", code = 1, count = cart.CartItems.Count };
            }
            return Json(code);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var code = new { success = false, msg = "", code = -1, count = 0 };

            Cart cart = (Cart)Session["Cart"];
            if (cart != null)
            {
                var checkProduct = cart.CartItems.FirstOrDefault(x => x.ProductId == id);
                if (checkProduct != null)
                {
                    cart.Remove(id);
                    code = new { success = true, msg = "", code = 1, count = cart.CartItems.Count };
                }

            }
            Session["Cart"] = cart;
            return Json(code);
        }

        [HttpPost]
        public ActionResult IncreaseQuantity(int id)
        {
            Cart cart = (Cart)Session["Cart"];
            var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
                cartItem.TotalPrice = cartItem.Quantity * cartItem.Price;
                Session["Cart"] = cart;
            }
            return Json(new { success = true, newQuantity = cartItem.Quantity, newTotalPrice = cartItem.TotalPrice, cartCount = cart.CartItems.Count });
        }

        [HttpPost]
        public ActionResult DecreaseQuantity(int id)
        {
            Cart cart = (Cart)Session["Cart"];
            var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == id);
            if (cartItem != null && cartItem.Quantity > 0)
            {
                cartItem.Quantity--;
                if (cartItem.Quantity == 0)
                    cart.CartItems.Remove(cartItem);
                else
                    cartItem.TotalPrice = cartItem.Quantity * cartItem.Price;

                Session["Cart"] = cart;
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

        [HttpPost]
        public ActionResult DeleteMany(List<int> ids)
        {
            var code = new { success = false, msg = "", code = -1, count = 0 };

            Cart cart = (Cart)Session["Cart"];
            if (cart != null && ids != null && ids.Any())
            {
                // Xóa tất cả sản phẩm có trong danh sách ids
                foreach (var id in ids)
                {
                    cart.Remove(id);
                }

                code = new { success = true, msg = "Items removed successfully", code = 1, count = cart.CartItems.Count };
            }

            Session["Cart"] = cart;
            return Json(code);
        }

    }
}