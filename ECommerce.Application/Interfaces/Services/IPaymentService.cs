using ECommerce.Application.DTO.Payment.Requests;
using ECommerce.Application.DTO.Payment.Responses;
using ECommerce.Domain.Shared;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<Result<PaymentResultDto>> CreateCheckoutSessionAsync(CreatePaymentRequest request);
    }
}
