using ECommerce.Application.DTO.Payment.Requests;
using ECommerce.Application.DTO.Payment.Responses;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Payment operations.
    /// </summary>
    [Authorize]
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public PaymentsController(IPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        /// <summary>
        /// Create Stripe checkout session.
        /// </summary>
        [HttpPost("checkout")]
        public async Task<ActionResult<PaymentResultDto>> Checkout([FromBody] CheckoutRequest request)
        {
            var order = await _orderService.GetOrderByIdAsync(request.OrderId, UserId);
            var domain = "http://localhost:3000/";

            var paymentRequest = new CreatePaymentRequest
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                Amount = order.TotalAmount,
                Currency = "usd",
                SuccessUrl = $"{domain}payment-success",
                CancelUrl = $"{domain}payment-cancel"
            };

            return Ok(await _paymentService.CreateCheckoutSessionAsync(paymentRequest));
        }
    }
}
