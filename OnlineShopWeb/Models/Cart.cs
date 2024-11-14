using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineShopWeb.Models
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual User Customer { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }

        public Cart()
        {
            this.CartItems = new List<CartItem>();
        }

        public void AddToCart(CartItem item, int quantity)
        {
            var checkExits = CartItems.FirstOrDefault(x => x.ProductId == item.ProductId);
            if (checkExits != null)
            {
                checkExits.Quantity += quantity;
                checkExits.TotalPrice = checkExits.Price * checkExits.Quantity;
            }
            else
            {
                CartItems.Add(item);
            }
        }

        public void Remove(int id)
        {
            var checkExits = CartItems.SingleOrDefault(x => x.ProductId == id);
            if (checkExits != null)
            {
                CartItems.Remove(checkExits);
            }
        }

        public void UpdateQuantity(int id, int quantity)
        {
            var checkExits = CartItems.SingleOrDefault(x => x.ProductId == id);
            if (checkExits != null)
            {
                checkExits.Quantity = quantity;
                checkExits.TotalPrice = checkExits.Price * checkExits.Quantity;
            }
        }

        public decimal GetTotalPrice()
        {
            return CartItems.Sum(x => x.TotalPrice);
        }

        public int GetTotalQuantity()
        {
            return CartItems.Sum(x => x.Quantity);
        }

        public void ClearCart()
        {
            CartItems.Clear();
        }
    }
}