using ECommerce.Application.DTO.Orders.Requests;
using ECommerce.Application.DTO.Orders.Responses;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Order management endpoints.
    /// </summary>
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Create a new order from the current user's cart.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrder()
        {
            var result = await _orderService.CreateOrderAsync(UserId);
            
            if (result.IsFailure)
            {
                return HandleResult(result);
            }

            return CreatedAtAction(nameof(GetOrderById), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Get all orders for the current user.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetUserOrders()
        {
            return HandleResult(await _orderService.GetUserOrdersAsync(UserId));
        }

        /// <summary>
        /// Get a specific order by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponse>> GetOrderById(int id)
        {
            return HandleResult(await _orderService.GetOrderByIdAsync(id, UserId));
        }

        /// <summary>
        /// Update order status (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<ActionResult<OrderResponse>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest updateDto)
        {
            return HandleResult(await _orderService.UpdateOrderStatusAsync(id, updateDto.Status));
        }

        /// <summary>
        /// Search and filter user orders.
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<OrderResponse>>> SearchUserOrders([FromQuery] OrderParams orderParams)
        {
            return HandleResult(await _orderService.SearchOrdersAsync(orderParams, UserId));
        }
    }
}
