using VposApi.Helpers;
using VposApi.Models;
using System.Security.Cryptography;
using System.Text;

public static class HashHelper
{
    public static string CalculateSHA512(string input, string secretKey)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hashBytes);
    }
    public static string GenerateRandomHex(int length)
    {
        const string chars = "ABCDEF0123456789";
        var sb = new StringBuilder();
        using var rng = RandomNumberGenerator.Create();
        byte[] buffer = new byte[1];

        while (sb.Length < length)
        {
            rng.GetBytes(buffer);
            char c = chars[buffer[0] % chars.Length];
            sb.Append(c);
        }

        return sb.ToString();
    }

    public static string BuildHashInput(PaymentRequest request)
    {
        string Safe(string s) => s ?? "";
        string FormatAmount(decimal? amount) => (amount ?? 0).ToString();
        string FormatInstallment(int? i) => (i ?? 0).ToString();

        return request.TransactionType switch
        {
            "SALEPOS" or "MAILORDER" or "PREAUTH" => string.Concat(
                Safe(request.UserId),
                Safe(request.Password),
                Safe(request.MerchantNumber),
                Safe(request.ShopCode),
                Safe(request.TransactionType),
                Safe(request.CardHolderName),
                Safe(request.Amount),
                Safe(request.Currency),
                Safe(request.Pan),
                Safe(request.Cvv2),
                Safe(request.ExpireDate),
                Safe(request.Installment),
                Safe(request.SecurityType),
                Safe(request.RewardAmount)
            ),

            "VOID" => string.Concat(
                Safe(request.UserId),
                Safe(request.Password),
                Safe(request.MerchantNumber),
                Safe(request.ShopCode),
                Safe(request.TransactionType),
                Safe(request.TransactionId),
                Safe(request.SecurityType)
            ),

            "REFUND" => string.Concat(
                Safe(request.UserId),
                Safe(request.Password),
                Safe(request.MerchantNumber),
                Safe(request.ShopCode),
                Safe(request.TransactionType),
                Safe(request.TransactionId),
                Safe(request.Amount),
                Safe(request.SecurityType)
            ),

            "MOTOINSURANCE" => string.Concat(
                Safe(request.UserId),
                Safe(request.Password),
                Safe(request.MerchantNumber),
                Safe(request.ShopCode),
                Safe(request.TransactionType),
                Safe(request.TransactionId),
                Safe(request.Amount),
                Safe(request.Currency),
                Safe(request.Bin),
                Safe(request.LastFourDigits),
                Safe(request.Installment),
                Safe(request.SecurityType)
            ),

            "POSTAUTH" => string.Concat(
                Safe(request.UserId),
                Safe(request.Password),
                Safe(request.MerchantNumber),
                Safe(request.ShopCode),
                Safe(request.TransactionType),
                Safe(request.TransactionId),
                Safe(request.Amount),
                Safe(request.SecurityType)
            ),

            _ => string.Empty
        };
    }


    public class HashInputResult
    {
        public string HashInput { get; set; }
        public string Timestamp { get; set; }
        public string RandomHex { get; set; }
    }


    public static HashInputResult BuildHashInputSecure(SecureRequest request)
    {
        HashInputResult hashInputResult = new HashInputResult();

        string Safe(string s) => s ?? "";
        hashInputResult.RandomHex = GenerateRandomHex(128);
        hashInputResult.Timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

        hashInputResult.HashInput = string.Concat(
                Safe(request.TransactionType),
                Safe(request.AcquirerMerchantId),
                Safe(request.OrderId),
                Safe(request.Pan),
                Safe(request.CardHolderName),
                Safe(request.ExpireDate),
                Safe(request.TransactionAmount),
                Safe(request.CurrencyCode),
                Safe(request.InstallmentCount),
                Safe(request.UserId),
                Safe(request.OkUrl),
                Safe(request.FailUrl),
                Safe(request.StoreType),
                hashInputResult.Timestamp,
                hashInputResult.RandomHex);

        return hashInputResult;
    }
}