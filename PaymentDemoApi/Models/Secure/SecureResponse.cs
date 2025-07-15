namespace VposApi.Models
{
    public class SecureResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseReasonCode { get; set; }
        public string ResponseMessage { get; set; }
        public string OrderId { get; set; }
        public string AuthorizationNumber { get; set; }
        public string RRN { get; set; }
        public string Stan { get; set; }
        public string TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsError { get; set; }
        public bool Success { get; set; }
    }
}
