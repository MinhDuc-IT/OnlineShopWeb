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

        [StringLength(50)]
        public string OrderCode { get; set; }

        public string OrderNotes { get; set; }

        [Required]
        public string PaymentMethod {  get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        [ForeignKey("CustomerId")]
        public virtual User Customer { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        // Trường lưu ảnh xác nhận đã giao hàng
        public string DeliveryConfirmationImage { get; set; } // Lưu đường dẫn ảnh xác nhận giao hàng

        // Trường hủy bởi ai và thời gian hủy
        public string CanceledBy { get; set; } // Lưu tên hoặc ID của người hủy
        public DateTime? CancellationTime { get; set; } // Lưu thời gian hủy

        // Trường xác nhận giao hàng thành công và thời gian
        public string DeliveredBy { get; set; } // Lưu tên hoặc ID của người xác nhận giao hàng thành công
        public DateTime? DeliveryConfirmationTime { get; set; } // Lưu thời gian xác nhận giao hàng

        // Trường thời gian vận chuyển
        public DateTime? ShippingTime { get; set; } // Lưu thời gian bắt đầu vận chuyển

        // Trường người xác nhận đơn
        public string OrderConfirmedBy { get; set; } // Lưu tên hoặc ID của người xác nhận đơn hàng
        public DateTime? OrderConfirmationTime { get; set; } // Lưu thời gian xác nhận đơn

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
        Delivered,

        [Description("Canceled")]
        Canceled,
    }

    public enum PaymentStatus
    {
        Pending, // Đang chờ thanh toán
        Completed, // Thanh toán thành công
        Refunded
    }
}