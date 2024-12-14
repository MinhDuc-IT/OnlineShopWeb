using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
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

        [Required(ErrorMessage = "Recipient name is required.")]
        [StringLength(100, ErrorMessage = "Recipient name must not exceed 100 characters.")]
        public string RecipientName { get; set; }

        [Required(ErrorMessage = "Recipient phone number is required.")]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Invalid phone number format.")]
        public string RecipientPhoneNumber { get; set; }

        [Required(ErrorMessage = "Recipient address is required.")]
        [StringLength(200, ErrorMessage = "Recipient address must not exceed 200 characters.")]
        public string RecipientAddress { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [ForeignKey("CustomerId")]
        public virtual User Customer { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    public enum OrderStatus
    {
        [Description("All")]
        All,

        [Description("Processing")]
        Processing,

        [Description("Shipping")]
        Shipping,

        [Description("Delivered")]
        Delivered
    }
}