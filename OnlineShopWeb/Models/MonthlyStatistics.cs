using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace OnlineShopWeb.Models
{
        public class MonthlyStatistics
        {
            public int Month { get; set; }
            public decimal Revenue { get; set; }
            public decimal Profit { get; set; }
        }
}