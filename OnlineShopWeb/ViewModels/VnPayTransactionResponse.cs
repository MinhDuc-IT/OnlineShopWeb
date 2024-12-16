using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShopWeb.ViewModels
{
    public class VnPayTransactionResponse
    {
        [JsonProperty("vnp_ResponseId")]
        public string VnpResponseId { get; set; }

        [JsonProperty("vnp_Command")]
        public string VnpCommand { get; set; }

        [JsonProperty("vnp_ResponseCode")]
        public string VnpResponseCode { get; set; }

        [JsonProperty("vnp_Message")]
        public string VnpMessage { get; set; }

        [JsonProperty("vnp_TmnCode")]
        public string VnpTmnCode { get; set; }

        [JsonProperty("vnp_TxnRef")]
        public string VnpTxnRef { get; set; }

        [JsonProperty("vnp_Amount")]
        public string VnpAmount { get; set; }

        [JsonProperty("vnp_OrderInfo")]
        public string VnpOrderInfo { get; set; }

        [JsonProperty("vnp_BankCode")]
        public string VnpBankCode { get; set; }

        [JsonProperty("vnp_PayDate")]
        public string VnpPayDate { get; set; }

        [JsonProperty("vnp_TransactionNo")]
        public string VnpTransactionNo { get; set; }

        [JsonProperty("vnp_TransactionType")]
        public string VnpTransactionType { get; set; }

        [JsonProperty("vnp_TransactionStatus")]
        public string VnpTransactionStatus { get; set; }

        [JsonProperty("vnp_SecureHash")]
        public string VnpSecureHash { get; set; }
    }
}