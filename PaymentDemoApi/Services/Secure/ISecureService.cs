using VposApi.Models;

namespace VposApi.Services
{
    public interface ISecureService
    {
        Task<SecureResponse> ProcessAsync(SecureRequest request);

        Task<PaymentResponse> Auth3DS(Auth3DSModel request);

    }
}
