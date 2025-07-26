namespace VposApi.Models
{
    public class SecureResponse
    {
        public string StoreType { get; set; }
        public string TransactionType { get; set; }
        public string OrderId { get; set; }
        public string Pan { get; set; }
        public string CardHolderName { get; set; }
        public string ExpireDate { get; set; }
        public string Cvv2 { get; set; }
        public string TransactionAmount { get; set; }
        public string CurrencyCode { get; set; }
        public string InstallmentCount { get; set; }
        public string AcquirerMerchantId { get; set; }
        public string UserId { get; set; }
        public string OkUrl { get; set; }
        public string FailUrl { get; set; }
        public string Hash { get; set; }
        public string Rnd { get; set; }
        public string TimeStamp { get; set; }
    }
}
