﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShopWeb.ViewModels
{
    public class CheckoutRequest
    {
        public int productId { get; set; }
        public int quantity { get; set; }
    }
}