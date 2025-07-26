namespace VposApi.Models
{
    public class Auth3DSModel
    {
        public string ApiVersion { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string HashData { get; set; } = string.Empty;
        public string MerchantNumber { get; set; } = string.Empty;
        public string ShopCode { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public string TransactionAmount { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public string Pan { get; set; } = string.Empty;
        public string Cvv2 { get; set; } = string.Empty;
        public string ExpireDate { get; set; } = string.Empty;
        public string InstallmentCount { get; set; } = string.Empty;
        public string SecurityType { get; set; } = string.Empty;
        public string ECI { get; set; } = string.Empty;
        public string Av { get; set; } = string.Empty;
        public string RewardAmount { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string AdditionalData { get; set; } = string.Empty;
        public string PFMerchantNumber { get; set; } = string.Empty;
        public string PFTaxNumber { get; set; } = string.Empty;
        public string CardBrand { get; set; } = string.Empty;
        public string BankIdentificationNumber { get; set; } = string.Empty;
        public string Last4Digit { get; set; } = string.Empty;
        public string CitizenshipNumber { get; set; } = string.Empty;
        public string ThreeDsProgramProtocol { get; set; } = string.Empty;
        public string ThreeDsDirectoryServerTransactionId { get; set; } = string.Empty;
        public string ThreeDModel { get; set; } = string.Empty;

    }
}
