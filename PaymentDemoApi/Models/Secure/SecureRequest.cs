using System.Text.Json.Serialization;

namespace VposApi.Models
{
    public class SecureRequest
    {
        [JsonPropertyName("transactionType")]
        public string TransactionType { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("pan")]
        public string Pan { get; set; }

        [JsonPropertyName("cardHolderName")]
        public string CardHolderName { get; set; }

        [JsonPropertyName("expireDate")]
        public string ExpireDate { get; set; }

        [JsonPropertyName("cvv2")]
        public string Cvv2 { get; set; }

        [JsonPropertyName("transactionAmount")]
        public string TransactionAmount { get; set; }

        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("installmentCount")]
        public string InstallmentCount { get; set; }

        [JsonPropertyName("acquirerMerchantId")]
        public string AcquirerMerchantId { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("okUrl")]
        public string OkUrl { get; set; }

        [JsonPropertyName("failUrl")]
        public string FailUrl { get; set; }

        [JsonPropertyName("storeType")]
        public string StoreType { get; set; }

        [JsonPropertyName("rnd")]
        public string Rnd { get; set; }

        [JsonPropertyName("linkPaymentToken")]
        public string LinkPaymentToken { get; set; }

        [JsonPropertyName("timeStamp")]
        public string TimeStamp { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }
    }
}
