using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Handles Stripe Webhook notifications.
    /// </summary>
    [ApiController]
    [Route("api/payments/webhook")]
    public class PaymentsWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentsWebhookController"/> class.
        /// </summary>
        public PaymentsWebhookController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Stripe Webhook entry point.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            var result = await _paymentService.FulfillPaymentAsync(json, stripeSignature!);

            if (result.IsFailure)
            {
                // Return 400 for signature verification failures so Stripe retries or logs the error
                return BadRequest(result.Error.Description);
            }

            return Ok();
        }
    }
}
