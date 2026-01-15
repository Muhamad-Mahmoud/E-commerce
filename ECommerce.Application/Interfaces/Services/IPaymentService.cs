using ECommerce.Application.DTO.Payment.Requests;
using ECommerce.Application.DTO.Payment.Responses;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentResultDto> CreateCheckoutSessionAsync(CreatePaymentRequest request);
    }
}
