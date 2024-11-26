using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineShopWeb.ViewModels
{
    public class OrderItem
    {
        public int ProductId { get; set; }
        public string Img { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;

        public OrderItem(int productId, string img, string name, decimal price, int quantity)
        {
            ProductId = productId;
            Img = img;
            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }
}