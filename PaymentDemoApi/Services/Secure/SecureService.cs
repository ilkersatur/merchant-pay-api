using Microsoft.Extensions.Options;
using VposApi.Models;
using System.Text;
using System.Text.Json;
using VposApi.Config;

namespace VposApi.Services
{
    public class SecureService : ISecureService
    {
        private readonly HttpClient _httpClient;
        private readonly VposSettings _settings;

        public SecureService(HttpClient httpClient, IOptions<VposSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<SecureResponse> ProcessAsync(SecureRequest request)
        {
            try
            {
                var inputString = HashHelper.BuildHashInputSecure(request);
                var hash = HashHelper.CalculateSHA512(inputString.HashInput, _settings.SecretKey);


                // Hash’i payload’a ekle
                var payload = new
                {
                    transactionType = request.TransactionType,
                    orderId = request.OrderId,
                    pan = request.Pan,
                    cardHolderName = request.CardHolderName,
                    expireDate = request.ExpireDate,
                    cvv2 = request.Cvv2,
                    transactionAmount = request.TransactionAmount,
                    currencyCode = request.CurrencyCode,
                    installmentCount = request.InstallmentCount,
                    acquirerMerchantId = request.AcquirerMerchantId,
                    userId = request.UserId,
                    okUrl = request.OkUrl,
                    failUrl = request.FailUrl,
                    storeType = request.StoreType,
                    rnd = inputString.RandomHex,
                    timeStamp = inputString.Timestamp,
                    hash = inputString.HashInput
                };

                string json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_settings.SecureEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    // Hata durumu için istersen burada özel işlem yapabilirsin
                    return new SecureResponse
                    {
                        Success = false,
                        ResponseMessage = "VPOS gateway isteği başarısız.",
                        TransactionDate = DateTime.UtcNow
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var gatewayResponse = JsonSerializer.Deserialize<SecureResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (gatewayResponse == null)
                {
                    return new SecureResponse
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
                return new SecureResponse
                {
                    Success = false
                };
            }
        }
    }
}
