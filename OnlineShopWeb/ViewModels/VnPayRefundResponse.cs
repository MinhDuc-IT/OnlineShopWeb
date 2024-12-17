using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShopWeb.ViewModels
{
    public class VnPayRefundResponse
    {
        public string Vnp_ResponseId { get; set; }      // Mã phản hồi từ hệ thống VNPay
        public string Vnp_Command { get; set; }         // Mã yêu cầu (refund)
        public string Vnp_TmnCode { get; set; }         // Mã định danh hệ thống của merchant
        public string Vnp_TxnRef { get; set; }          // Mã tham chiếu giao dịch của hệ thống merchant
        public decimal Vnp_Amount { get; set; }         // Số tiền hoàn
        public string Vnp_OrderInfo { get; set; }       // Thông tin đơn hàng
        public string Vnp_ResponseCode { get; set; }    // Mã phản hồi kết quả (ví dụ "00" thành công)
        public string Vnp_Message { get; set; }         // Mô tả kết quả của mã phản hồi
        public string Vnp_BankCode { get; set; }        // Mã ngân hàng (hoặc ví điện tử)
        public string Vnp_PayDate { get; set; }         // Ngày thực hiện giao dịch hoàn tiền
        public string Vnp_TransactionNo { get; set; }   // Mã giao dịch hoàn tiền
        public string Vnp_TransactionType { get; set; } // Loại giao dịch (02 - hoàn trả toàn phần, 03 - hoàn trả một phần)
        public string Vnp_TransactionStatus { get; set; } // Trạng thái giao dịch hoàn tiền
        public string Vnp_SecureHash { get; set; }      // Mã kiểm tra dữ liệu (checksum)
    }

}