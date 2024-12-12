using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int BrandId { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Click count must be a non-negative integer.")]
        public int Click { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal CostPrice { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<UserProduct> UserProducts { get; set; }
    }
}