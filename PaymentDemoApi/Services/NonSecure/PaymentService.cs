using Microsoft.Extensions.Options;
using VposApi.Models;
using System.Text;
using System.Text.Json;
using VposApi.Config;
using VposApi.Helpers;

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
                request.CardBrand = "M";
                request.RewardAmount = "0";

                var inputString = HashHelper.BuildHashInput(request);
                var hash = HashHelper.CalculateSHA512(inputString, _settings.SecretKey);

                var payload = new object();

                switch (request.TransactionType)
                {
                    case "SALEPOS":
                    case "PREAUTH":
                    case "MAILORDER":
                        payload = PrepareSaleRequest(request, hash);
                        break;

                    case "VOID":
                        payload = PrepareVoidRequest(request, hash);
                        break;

                    case "REFUND":
                        payload = PrepareRefundRequest(request, hash);
                        break;

                    case "POSTAUTH":
                        payload = PreparePostAuthRequest(request, hash);
                        break;

                    case "MOTOINSURANCE":
                        payload = PrepareMotoInsuranceRequest(request, hash);
                        break;

                    default:
                        // İsteğe bağlı: geçersiz işlem tipi için hata fırlat
                        throw new InvalidOperationException($"Geçersiz işlem türü: {request.TransactionType}");
                }

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

                string respJson = JsonSerializer.Serialize(gatewayResponse, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(respJson);

                if (int.TryParse(gatewayResponse.ResponseMessage, out int msgNum))
                {
                    if (gatewayResponse.ResponseMessage.Length == 6)
                        gatewayResponse.ResponseMessage += "-" + InternalResponseMapping.GetInternalResponseMappingList(gatewayResponse.ResponseMessage);
                    else if (gatewayResponse.ResponseMessage.Length == 4)
                        gatewayResponse.ResponseMessage += "-" + ResponseCodeDef.GetResponseCodeDefList(gatewayResponse.ResponseMessage);
                }
                else
                {
                    gatewayResponse.ResponseMessage += "-" + ResponseReasonCodeDef.GetResponseReasonCodeList(gatewayResponse.ResponseMessage);
                }

                if (int.TryParse(gatewayResponse.ResponseCode, out int codeNum))
                {
                    if (gatewayResponse.ResponseCode.Length == 6)
                        gatewayResponse.ResponseCode += "-" + InternalResponseMapping.GetInternalResponseMappingList(gatewayResponse.ResponseCode);
                    else if (gatewayResponse.ResponseCode.Length == 4)
                        gatewayResponse.ResponseCode += "-" + ResponseCodeDef.GetResponseCodeDefList(gatewayResponse.ResponseCode);
                }
                else
                {
                    gatewayResponse.ResponseCode += "-" + ResponseReasonCodeDef.GetResponseReasonCodeList(gatewayResponse.ResponseCode);
                }

                if (int.TryParse(gatewayResponse.ResponseReasonCode, out int reasonCodeNum))
                {
                    if (gatewayResponse.ResponseReasonCode.Length == 6)
                        gatewayResponse.ResponseReasonCode += "-" + InternalResponseMapping.GetInternalResponseMappingList(gatewayResponse.ResponseReasonCode);
                    else if (gatewayResponse.ResponseReasonCode.Length == 4)
                        gatewayResponse.ResponseReasonCode += "-" + ResponseCodeDef.GetResponseCodeDefList(gatewayResponse.ResponseReasonCode);
                }
                else
                {
                    if (gatewayResponse.ResponseReasonCode is not null)
                    {
                        gatewayResponse.ResponseReasonCode += "-" + ResponseReasonCodeDef.GetResponseReasonCodeList(gatewayResponse.ResponseReasonCode);
                    }
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

        public object PrepareSaleRequest(PaymentRequest request, string hash)
        {
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
                apiKey = _settings.ApiKey,
                iPAddress = _settings.IPAddress,
                hashData = hash
            };

            return payload;
        }

        public object PrepareVoidRequest(PaymentRequest request, string hash)
        {
            var payload = new
            {
                userId = request.UserId,
                password = request.Password,
                merchantNumber = request.MerchantNumber,
                shopCode = request.ShopCode,
                transactionType = request.TransactionType,
                transactionId = request.TransactionId,
                securityType = request.SecurityType,
                iPAddress = _settings.IPAddress,
                hashData = hash
            };

            return payload;
        }

        public object PrepareRefundRequest(PaymentRequest request, string hash)
        {
            var payload = new
            {
                userId = request.UserId,
                password = request.Password,
                merchantNumber = request.MerchantNumber,
                shopCode = request.ShopCode,
                transactionType = request.TransactionType,
                transactionId = request.TransactionId,
                refundAmount = request.Amount,
                securityType = request.SecurityType,
                iPAddress = _settings.IPAddress,
                hashData = hash
            };

            return payload;
        }

        public object PreparePostAuthRequest(PaymentRequest request, string hash)
        {
            var payload = new
            {
                userId = request.UserId,
                password = request.Password,
                merchantNumber = request.MerchantNumber,
                shopCode = request.ShopCode,
                transactionType = request.TransactionType,
                transactionId = request.TransactionId,
                transactionAmount = request.Amount,
                currencyCode = request.Currency,
                securityType = request.SecurityType,
                iPAddress = _settings.IPAddress,
                hashData = hash
            };

            return payload;
        }

        public object PrepareMotoInsuranceRequest(PaymentRequest request, string hash)
        {
            var payload = new
            {
                userId = request.UserId,
                password = request.Password,
                merchantNumber = request.MerchantNumber,
                shopCode = request.ShopCode,
                transactionType = request.TransactionType,
                transactionAmount = request.Amount,
                currencyCode = request.Currency,
                installmentCount = request.Installment,
                securityType = request.SecurityType,
                bin = request.Bin,
                lastFourDigits = request.LastFourDigits,
                tcknVkn = request.TcknVkn,
                rewardAmount = request.RewardAmount,
                iPAddress = _settings.IPAddress,
                hashData = hash
            };

            return payload;
        }
    }
}
