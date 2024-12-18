using OnlineShopWeb.ViewModels;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;

namespace OnlineShopWeb.Helpers
{
    public class VnPayService
    {
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", ConfigurationManager.AppSettings["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", ConfigurationManager.AppSettings["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", ConfigurationManager.AppSettings["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", ConfigurationManager.AppSettings["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", ConfigurationManager.AppSettings["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng:" + model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", ConfigurationManager.AppSettings["VnPay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick);
            vnpay.AddRequestData("vnp_BankCode", "NCB");

            var paymentUrl = vnpay.CreateRequestUrl(ConfigurationManager.AppSettings["VnPay:BaseUrl"], ConfigurationManager.AppSettings["VnPay:HashSecret"]);

            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(NameValueCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (string key in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, collections[key]);
                }
            }

            var vnp_orderCode = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_amount = Convert.ToDouble(vnpay.GetResponseData("vnp_Amount"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections["vnp_SecureHash"];
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var vnp_PayDate = Convert.ToInt64(vnpay.GetResponseData("vnp_PayDate"));
            var vnp_BankCode = vnpay.GetResponseData("vnp_BankCode");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, ConfigurationManager.AppSettings["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = vnp_BankCode,
                OrderDescription = vnp_OrderInfo,
                OrderCode = vnp_orderCode.ToString(),
                Amount = vnp_amount,
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode,
                TransactionDate = vnp_PayDate.ToString(),
            };
        }
    }
}
