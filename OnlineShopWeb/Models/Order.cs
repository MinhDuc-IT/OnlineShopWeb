using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineShopWeb.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal ToTalAmount { get; set; }

        [ForeignKey("CustomerId")]
        public virtual User Customer { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}