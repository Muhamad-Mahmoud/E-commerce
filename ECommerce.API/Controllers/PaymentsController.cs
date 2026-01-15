using System.Security.Claims;
using ECommerce.Application.DTO.Payment.Requests;
using ECommerce.Application.DTO.Payment.Responses;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            IPaymentService paymentService, 
            IOrderService orderService, 
            ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Initiates a Stripe checkout session for an order.
        /// </summary>
        /// <param name="request">The checkout request containing the Order ID.</param>
        /// <returns>The checkout session URL.</returns>
        [HttpPost("checkout")]
        public async Task<ActionResult<PaymentResultDto>> Checkout([FromBody] CheckoutRequest request)
        {
            try
            {
                var userId = GetUserId();
                var order = await _orderService.GetOrderByIdAsync(request.OrderId, userId);

                // From Front End
                var domain = $"http://localhost:3000/";

                var paymentRequest = new CreatePaymentRequest
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    Amount = order.TotalAmount,
                    Currency = "usd",
                    SuccessUrl = $"{domain}payment-success",
                    CancelUrl = $"{domain}payment-cancel"
                };

                var result = await _paymentService.CreateCheckoutSessionAsync(paymentRequest);

                _logger.LogInformation("Payment session created for order {OrderId} by user {UserId}", request.OrderId, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Order not found." });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment session for order {OrderId}", request.OrderId);
                return StatusCode(500, new { message = "An error occurred while initiating payment." });
            }
        }

        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User ID not found in claims");
            return userId;
        }
    }
}
