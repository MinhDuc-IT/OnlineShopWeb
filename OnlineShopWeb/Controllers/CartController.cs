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
        private ApplicationDbContext db = new ApplicationDbContext();

        public void CheckAndInsertProduct()
        {
            int productCount = db.Products.Count();

            if (productCount < 3)
            {
                var newProduct = new Product
                {
                    Name = "test",
                    BrandId = 1,
                    Description = "test",
                    Image = "https://placehold.co/90x90?text=OrderItem",
                    Price = 40000,
                    Stock = 9999,
                    CategoryId = 0,
                    IsDeleted = false,
                    CostPrice = 25000
                };
                var newProduct1 = new Product
                {
                    Name = "test1",
                    BrandId = 1,
                    Description = "test",
                    Image = "https://placehold.co/90x90?text=OrderItem",
                    Price = 40000,
                    Stock = 9999,
                    CategoryId = 0,
                    IsDeleted = false,
                    CostPrice = 25000
                };
                var newProduct2 = new Product
                {
                    Name = "test2",
                    BrandId = 1,
                    Description = "test",
                    Image = "https://placehold.co/90x90?text=OrderItem",
                    Price = 40000,
                    Stock = 9999,
                    CategoryId = 0,
                    IsDeleted = false,
                    CostPrice = 25000
                };

                db.Products.AddRange(new List<Product> { newProduct,newProduct1,newProduct2});

                db.SaveChanges();
            }
        }

        // GET: Cart
        public ActionResult Index()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart == null)
            {
                var cartItems = new List<CartItem>();

                int defaultQuantity = 1;

                var product = db.Products.FirstOrDefault(p => p.Name == "test");
                var product1 = db.Products.FirstOrDefault(p => p.Name == "test1");
                var product2 = db.Products.FirstOrDefault(p => p.Name == "test2");

                var item = new CartItem()
                {
                    ProductId = product.ProductId,
                    Product = product,
                    Quantity = defaultQuantity,
                    Price = product.Price
                };

                var item1 = new CartItem()
                {
                    ProductId = product1.ProductId,
                    Product = product1,
                    Quantity = defaultQuantity,
                    Price = product1.Price
                };
                var item2 = new CartItem()
                {
                    ProductId = product2.ProductId,
                    Product = product2,
                    Quantity = defaultQuantity,
                    Price = product2.Price
                };

                item.TotalPrice = item.Price * item.Quantity;
                item1.TotalPrice = item1.Price * item1.Quantity;
                item2.TotalPrice = item2.Price * item2.Quantity;


                cartItems.Add(item);
                cartItems.Add(item1);
                cartItems.Add(item2);

                cart = new Cart
                {
                    CartItems = cartItems
                };
                Session["Cart"] = cart;
            }
            return View(cart.CartItems);
        }

        public ActionResult ShowCount()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.CartItems.Count }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }

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