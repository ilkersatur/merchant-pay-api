using Microsoft.Extensions.Options;
using VposApi.Models;
using System.Text;
using System.Text.Json;
using VposApi.Config;

namespace VposApi.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly VposSettings _settings;

        public PaymentService(HttpClient httpClient, IOptions<VposSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<PaymentResponse> ProcessAsync(PaymentRequest request)
        {
            try
            {
                var inputString = HashHelper.BuildHashInput(request);
                var hash = HashHelper.CalculateSHA512(inputString, _settings.SecretKey);


                // Hash’i payload’a ekle
                var payload = new
                {
                    userId = request.UserId,
                    password = request.Password,
                    merchantNumber = request.MerchantNumber,
                    shopCode = request.ShopCode,
                    transactionType = request.TransactionType,
                    transactionId = request.TransactionId,
                    cardHolderName = request.CardHolderName,
                    transactionAmount = request.Amount,
                    currencyCode = request.Currency,
                    pan = request.Pan,
                    cvv2 = request.Cvv2,
                    expireDate = request.ExpireDate,
                    installmentCount = request.Installment,
                    securityType = request.SecurityType,
                    rewardAmount = request.RewardAmount,
                    pfMerchantNumber = request.PFMerchantNumber,
                    cardBrand = request.CardBrand,
                    bin = request.Bin,
                    lastFourDigits = request.LastFourDigits,
                    tcknVkn = request.TcknVkn,
                    apiKey = _settings.ApiKey,
                    iPAddress = _settings.IPAddress,
                    hashData = hash
                };

                string json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_settings.NonSecureEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    // Hata durumu için istersen burada özel işlem yapabilirsin
                    return new PaymentResponse
                    {
                        Success = false,
                        ResponseMessage = "VPOS gateway isteği başarısız.",
                        TransactionDate = DateTime.UtcNow
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var gatewayResponse = JsonSerializer.Deserialize<PaymentResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (gatewayResponse == null)
                {
                    return new PaymentResponse
                    {
                        Success = false,
                        ResponseMessage = "Gateway'den boş cevap alındı.",
                        TransactionDate = DateTime.UtcNow
                    };
                }

                return gatewayResponse;
            }
            catch (Exception ex)
            {
                return new PaymentResponse
                {
                    Success = false
                };
            }
        }
    }
}
