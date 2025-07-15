using System.Text.Json.Serialization;

namespace VposApi.Models
{
    public class PaymentRequest
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("merchantNumber")]
        public string MerchantNumber { get; set; }

        [JsonPropertyName("shopCode")]
        public string ShopCode { get; set; }

        [JsonPropertyName("transactionType")]
        public string TransactionType { get; set; }

        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; }

        [JsonPropertyName("cardHolderName")]
        public string CardHolderName { get; set; }

        [JsonPropertyName("transactionAmount")]
        public string Amount { get; set; }

        [JsonPropertyName("currencyCode")]
        public string Currency { get; set; }

        [JsonPropertyName("pan")]
        public string Pan { get; set; }

        [JsonPropertyName("cvv2")]
        public string Cvv2 { get; set; }

        [JsonPropertyName("expireDate")]
        public string ExpireDate { get; set; }

        [JsonPropertyName("installmentCount")]
        public string Installment { get; set; }

        [JsonPropertyName("securityType")]
        public string SecurityType { get; set; }

        [JsonPropertyName("rewardAmount")]
        public string RewardAmount { get; set; }

        [JsonPropertyName("pfMerchantNumber")]
        public string PFMerchantNumber { get; set; }

        [JsonPropertyName("cardBrand")]
        public string CardBrand { get; set; }

        [JsonPropertyName("bin")]
        public string Bin { get; set; }

        [JsonPropertyName("lastFourDigits")]
        public string LastFourDigits { get; set; }

        [JsonPropertyName("tcknVkn")]
        public string TcknVkn { get; set; }
    }
}
