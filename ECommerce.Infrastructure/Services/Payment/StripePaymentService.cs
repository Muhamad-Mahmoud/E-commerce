using ECommerce.Application.DTO.Payment.Requests;
using ECommerce.Application.DTO.Payment.Responses;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Exceptions;
using ECommerce.Infrastructure.Helper;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace ECommerce.Infrastructure.Services.Payment
{
    public class StripePaymentService : IPaymentService
    {
        private readonly StripeSettings _stripeSettings;

        public StripePaymentService(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.Secretkey;
        }

        public async Task<Result<PaymentResultDto>> CreateCheckoutSessionAsync(CreatePaymentRequest request)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(request.Amount * 100),
                            Currency = request.Currency,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Order #{request.OrderNumber}",
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl,
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", request.OrderId.ToString() }
                }
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Result.Success(new PaymentResultDto
            {
                SessionId = session.Id,
                Url = session.Url,
                PaymentStatus = session.PaymentStatus
            });
        }
    }
}
