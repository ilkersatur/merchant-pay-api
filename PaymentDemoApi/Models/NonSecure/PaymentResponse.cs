namespace VposApi.Models
{
    public class PaymentResponse
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
    public class GatewayResponse
    {
        public AuthenticationResult AuthenticationResult { get; set; }
        public Result Result { get; set; }
        public TransactionResponse TransactionResponse { get; set; }
    }

    public class AuthenticationResult
    {
        public string AuthorizationNumber { get; set; }
        public string RRN { get; set; }
        public string Stan { get; set; }
    }
    public class Result
    {
        public string Code { get; set; }
        public string ReasonCode { get; set; }
        public string Message { get; set; }
    }
    public class TransactionResponse
    {
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
