using System;
using System.Collections.Generic;

namespace OnlineShopWeb.ViewModels
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }  // Phương thức thanh toán
        public decimal TotalAmount { get; set; }
        public string OrderNotes { get; set; }  // Ghi chú cho đơn hàng
        public string RecipientName { get; set; } // Tên người nhận
        public string RecipientPhoneNumber { get; set; } // Số điện thoại người nhận
        public string RecipientAddress { get; set; } // Địa chỉ người nhận
        public string DeliveredBy { get; set; } // Người giao hàng
        public DateTime? DeliveryConfirmationTime { get; set; } // Thời gian giao hàng
        public string CanceledBy { get; set; } // Người hủy đơn
        public DateTime? CancellationTime { get; set; } // Thời gian hủy
        public DateTime? OrderConfirmationTime { get; set; } // Thời gian vận chuyển

        public List<OrderDetailDTO> OrderDetails { get; set; }
    }

    public class OrderDetailDTO
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ProductImage { get; set; }  // Hình ảnh của sản phẩm
    }
}
