using VposApi.Models;

namespace VposApi.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessAsync(PaymentRequest request);

    }
}
