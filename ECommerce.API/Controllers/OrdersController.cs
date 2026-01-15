using System.Security.Claims;
using ECommerce.Application.DTO.Orders.Requests;
using ECommerce.Application.DTO.Orders.Responses;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Manages order operations for authenticated users.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new order from the user's shopping cart.
        /// </summary>
        /// <returns>The created order.</returns>
        /// <response code="201">Order created successfully, returns the order details.</response>
        /// <response code="400">Invalid request or shopping cart is empty.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrder()
        {
            try
            {
                var userId = GetUserId();
                var order = await _orderService.CreateOrderAsync(userId);
                _logger.LogInformation("Order created successfully for user {UserId}", userId);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Invalid operation when creating order: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user {UserId}", GetUserId());
                return StatusCode(500, new { message = "An error occurred while creating the order." });
            }
        }

        /// <summary>
        /// Retrieves all orders for the authenticated user.
        /// </summary>
        /// <returns>A list of the user's orders.</returns>
        /// <response code="200">Returns the list of user's orders.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetUserOrders()
        {
            try
            {
                var userId = GetUserId();
                var orders = await _orderService.GetUserOrdersAsync(userId);
                return Ok(orders);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid user ID: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific order by ID with all related details.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <returns>The order details.</returns>
        /// <response code="200">Returns the order.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        /// <response code="403">Forbidden - User does not own this order.</response>
        /// <response code="404">Order not found.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponse>> GetOrderById(int id)
        {
            try
            {
                var userId = GetUserId();
                var order = await _orderService.GetOrderByIdAsync(id, userId);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Order {OrderId} not found", id);
                return NotFound(new { message = "Order not found." });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized access attempt to order {OrderId} by user {UserId}",
                    id, GetUserId());
                return Forbid();
            }
        }

        /// <summary>
        /// Updates the status of an order. Admin only.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <param name="updateDto">The new order status.</param>
        /// <returns>The updated order.</returns>
        /// <response code="200">Order status updated successfully, returns the updated order.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        /// <response code="403">Forbidden - User does not have Admin role.</response>
        /// <response code="404">Order not found.</response>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderResponse>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest updateDto)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, updateDto.Status);
                _logger.LogInformation("Order {OrderId} status updated to {NewStatus}", id, updateDto.Status);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Attempt to update status for non-existent order {OrderId}", id);
                return NotFound(new { message = "Order not found." });
            }
        }

        /// <summary>
        /// Searches user's orders with pagination, filtering, and sorting.
        /// </summary>
        /// <param name="orderParams">Pagination, filtering, and sorting parameters.</param>
        /// <returns>A paginated list of orders matching the criteria.</returns>
        /// <response code="200">Returns the paginated search results.</response>
        /// <response code="400">Invalid request parameters.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<OrderResponse>>> SearchUserOrders([FromQuery] OrderParams orderParams)
        {
            try
            {
                var userId = GetUserId();
                var results = await _orderService.SearchOrdersAsync(orderParams, userId);
                return Ok(results);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid search parameters: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching orders for user {UserId}", GetUserId());
                return StatusCode(500, new { message = "An error occurred while searching orders." });
            }
        }

        /// <summary>
        /// Extracts the user ID from the current claims.
        /// </summary>
        /// <returns>The user ID.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when user ID is not found in claims.</exception>
        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User ID not found in claims");
            return userId;
        }
    }
}
