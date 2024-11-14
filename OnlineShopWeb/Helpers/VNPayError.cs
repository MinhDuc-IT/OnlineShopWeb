using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShopWeb.Helpers
{
    public static class VNPayError
    {
        private static readonly Dictionary<string, string> ErrorMessages = new Dictionary<string, string>
    {
        { "00", "Giao dịch thành công" },
        { "07", "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường)." },
        { "09", "Giao dịch không thành công: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking." },
        { "10", "Giao dịch không thành công: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần." },
        { "11", "Giao dịch không thành công: Đã hết hạn chờ thanh toán. Vui lòng thử lại." },
        { "12", "Giao dịch không thành công: Thẻ/Tài khoản của khách hàng bị khóa." },
        { "13", "Giao dịch không thành công: Sai mật khẩu OTP. Vui lòng thử lại." },
        { "24", "Giao dịch không thành công: Khách hàng hủy giao dịch." },
        { "51", "Giao dịch không thành công: Tài khoản không đủ số dư." },
        { "65", "Giao dịch không thành công: Vượt quá hạn mức giao dịch trong ngày." },
        { "75", "Ngân hàng thanh toán đang bảo trì." },
        { "79", "Giao dịch không thành công: Nhập sai mật khẩu thanh toán quá số lần quy định." },
        { "99", "Lỗi không xác định hoặc các lỗi khác." }
    };

        public static string GetMessage(string responseCode)
        {
            return ErrorMessages.TryGetValue(responseCode, out var message) ? message : "Lỗi thanh toán không xác định.";
        }
    }
}