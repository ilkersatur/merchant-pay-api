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

                var gatewayResponse = JsonSerializer.Deserialize<GatewayResponse>(responseContent, new JsonSerializerOptions
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

                if (int.TryParse(gatewayResponse.Result.Message, out int msgNum))
                {
                    if (gatewayResponse.Result.Message.Length == 6)
                        gatewayResponse.Result.Message += "-" + InternalResponseMapping.GetInternalResponseMappingList(gatewayResponse.Result.Message);
                    else if (gatewayResponse.Result.Message.Length == 4)
                        gatewayResponse.Result.Message += "-" + ResponseCodeDef.GetResponseCodeDefList(gatewayResponse.Result.Message);
                }
                else
                {
                    gatewayResponse.Result.Message += "-" + ResponseReasonCodeDef.GetResponseReasonCodeList(gatewayResponse.Result.Message);
                }

                if (int.TryParse(gatewayResponse.Result.Code, out int codeNum))
                {
                    if (gatewayResponse.Result.Code.Length == 6)
                        gatewayResponse.Result.Code += "-" + InternalResponseMapping.GetInternalResponseMappingList(gatewayResponse.Result.Code);
                    else if (gatewayResponse.Result.Code.Length == 4)
                        gatewayResponse.Result.Code += "-" + ResponseCodeDef.GetResponseCodeDefList(gatewayResponse.Result.Code);
                }
                else
                {
                    gatewayResponse.Result.Code += "-" + ResponseReasonCodeDef.GetResponseReasonCodeList(gatewayResponse.Result.Code);
                }

                if (int.TryParse(gatewayResponse.Result.ReasonCode, out int reasonCodeNum))
                {
                    if (gatewayResponse.Result.ReasonCode.Length == 6)
                        gatewayResponse.Result.ReasonCode += "-" + InternalResponseMapping.GetInternalResponseMappingList(gatewayResponse.Result.ReasonCode);
                    else if (gatewayResponse.Result.ReasonCode.Length == 4)
                        gatewayResponse.Result.ReasonCode += "-" + ResponseCodeDef.GetResponseCodeDefList(gatewayResponse.Result.ReasonCode);
                }
                else
                {
                    gatewayResponse.Result.ReasonCode += "-" + ResponseReasonCodeDef.GetResponseReasonCodeList(gatewayResponse.Result.ReasonCode);
                }

                PaymentResponse paymentResponse = new()
                {
                    AuthorizationNumber = gatewayResponse.AuthenticationResult.AuthorizationNumber,
                    RRN = gatewayResponse.AuthenticationResult.RRN,
                    Stan = gatewayResponse.AuthenticationResult.Stan,
                    TransactionId = gatewayResponse.TransactionResponse.TransactionId,
                    TransactionDate = gatewayResponse.TransactionResponse.TransactionDate,
                    OrderId = gatewayResponse.TransactionResponse.OrderId,
                    ResponseCode = gatewayResponse.Result.Code,
                    ResponseMessage = gatewayResponse.Result.Message,
                    ResponseReasonCode = gatewayResponse.Result.ReasonCode,
                    Success = gatewayResponse.Result.Code == "000000-SUCCESSFUL" ? true : false,
                    IsError = gatewayResponse.Result.Code == "000000-SUCCESSFUL" ? false : true,
                };

                return paymentResponse;
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
