using System;
using System.Collections.Generic;

namespace OnlineShopWeb.ViewModels
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }  // Thêm phương thức thanh toán
        public decimal TotalAmount { get; set; }
        public string OrderNotes { get; set; }  // Thêm ghi chú cho đơn hàng
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
