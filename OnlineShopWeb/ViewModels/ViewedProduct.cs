using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShopWeb.ViewModels
{
    public class ViewedProduct
    {
        public int ProductId { get; set; }
        public int ViewCount { get; set; }
    }

    public class OrderedProduct
    {
        public int ProductId { get; set; }
        public int OrderCount { get; set; }
    }

    public class HotProduct
    {
        public int ProductId { get; set; }
        public double HotScore { get; set; }
    }

}