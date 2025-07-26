using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using VposApi.Config;
using VposApi.Models;
using VposApi.Helpers;

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
                request.OkUrl = _settings.OkUrl;
                request.FailUrl = _settings.FailUrl;

                if (request.StoreType == "3D_PAY" || request.StoreType == "3D_PAY_HOSTING")
                {
                    request.OkUrl = _settings.TDPayOkUrl;
                    request.FailUrl = _settings.TDPayFailUrl;
                }

                var inputString = HashHelper.BuildHashInputSecure(request);
                var hash = HashHelper.CalculateSHA512(inputString.HashInput, _settings.SecretKey);

                return new()
                {
                    Hash = hash,
                    Rnd = inputString.RandomHex,
                    TimeStamp = inputString.Timestamp,
                    AcquirerMerchantId = request.AcquirerMerchantId,
                    CardHolderName = request.CardHolderName,
                    CurrencyCode = request.CurrencyCode,
                    Cvv2 = request.Cvv2,
                    ExpireDate = request.ExpireDate,
                    OkUrl = request.OkUrl,
                    FailUrl = request.FailUrl,
                    InstallmentCount = request.InstallmentCount,
                    OrderId = request.OrderId,
                    Pan = request.Pan,
                    StoreType = request.StoreType,
                    TransactionAmount = request.TransactionAmount,
                    TransactionType = request.TransactionType,
                    UserId = request.UserId
                };

            }
            catch (Exception ex)
            {
                return new SecureResponse
                {

                };
            }
        }

        public async Task<PaymentResponse> Auth3DS(Auth3DSModel request)
        {
            try
            {
                request.OrderId = string.IsNullOrEmpty(request.OrderId) ? Guid.NewGuid().ToString().ToUpper() : request.OrderId.ToUpper();
                request.CardBrand = "M";
                request.RewardAmount = "0";

                var inputString = HashHelper.BuildHashInputAuth3DS(request);
                var hash = HashHelper.CalculateSHA512(inputString, _settings.SecretKey);

                // Hash’i payload’a ekle
                var payload = new
                {
                    av = request.Av,
                    cardBrand = request.CardBrand,
                    currencyCode = request.CurrencyCode,
                    eci = request.ECI,
                    hashData = hash,
                    iPAddress = _settings.IPAddress,
                    installmentCount = request.InstallmentCount,
                    merchantNumber = request.MerchantNumber,
                    orderId = request.OrderId,
                    password = request.Password,
                    userId = request.UserId,
                    shopCode = request.ShopCode,
                    storeType = request.ThreeDModel,
                    threeDsDirectoryServerTransactionId = request.ThreeDsDirectoryServerTransactionId,
                    threeDsProgramProtocol = request.ThreeDsProgramProtocol,
                    transactionType = request.TransactionType,
                    transactionId = request.TransactionId,
                    transactionAmount = request.TransactionAmount,
                    securityType = request.ThreeDModel,
                    rewardAmount = request.RewardAmount

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


                string respJson = JsonSerializer.Serialize(gatewayResponse, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(respJson);

                /////////////////////

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
                    gatewayResponse.ResponseReasonCode += "-" + ResponseReasonCodeDef.GetResponseReasonCodeList(gatewayResponse.ResponseReasonCode);
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
