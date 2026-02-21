using ECommerce.Application.DTO.Payment.Requests;
using ECommerce.Application.DTO.Payment.Responses;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<Result<PaymentResultDto>> CreateCheckoutSessionAsync(CreatePaymentRequest request);
        Task<Result> FulfillPaymentAsync(string json, string stripeSignature);
    }
}
