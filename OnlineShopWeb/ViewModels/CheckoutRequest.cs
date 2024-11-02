using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShopWeb.ViewModels
{
    public class CheckoutRequest
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string OrderNotes { get; set; }
        public double Total { get; set; }
        public string PaymentMethod { get; set; } = "COD";

    }
}