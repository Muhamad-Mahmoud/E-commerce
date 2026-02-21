using ECommerce.Application.DTO.Payment.Requests;
using ECommerce.Application.DTO.Payment.Responses;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Helper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace ECommerce.Infrastructure.Services.Payment
{
    public class StripePaymentService : IPaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StripePaymentService> _logger;

        public StripePaymentService(
            IOptions<StripeSettings> stripeSettings, 
            IUnitOfWork unitOfWork, 
            ILogger<StripePaymentService> logger)
        {
            _stripeSettings = stripeSettings.Value;
            _unitOfWork = unitOfWork;
            _logger = logger;
            StripeConfiguration.ApiKey = _stripeSettings.Secretkey;
        }

        public async Task<Result<PaymentResultDto>> CreateCheckoutSessionAsync(CreatePaymentRequest request)
        {
            try 
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
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error creating checkout session");
                return Result.Failure<PaymentResultDto>(new Error("Payment.ProviderError", "An error occurred with the payment provider.", 503));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating checkout session");
                return Result.Failure<PaymentResultDto>(DomainErrors.General.ServerError);
            }
        }

        public async Task<Result> FulfillPaymentAsync(string json, string stripeSignature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _stripeSettings.WebhookSecret);

                if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session != null && session.Metadata.TryGetValue("OrderId", out var orderIdStr) && int.TryParse(orderIdStr, out var orderId))
                    {
                        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                        if (order == null)
                        {
                            _logger.LogWarning("Order {OrderId} not found for webhook processing", orderId);
                            return Result.Failure(DomainErrors.Order.NotFound);
                        }

                        if (session.PaymentStatus == "paid")
                        {
                            order.UpdatePaymentStatus(PaymentStatus.Paid);
                            order.UpdateStatus(OrderStatus.Processing);
                            _unitOfWork.Orders.Update(order);
                            await _unitOfWork.SaveChangesAsync();
                            
                            _logger.LogInformation("Order {OrderId} successfully paid and updated via webhook", orderId);
                        }
                    }
                }

                return Result.Success();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook signature verification failed");
                return Result.Failure(new Error("Payment.WebhookError", "Webhook signature verification failed.", 400));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook");
                return Result.Failure(DomainErrors.General.ServerError);
            }
        }
    }
}
