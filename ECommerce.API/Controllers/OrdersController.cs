using ECommerce.Application.DTO.Orders.Requests;
using ECommerce.Application.DTO.Orders.Responses;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// User orders management.
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
        /// Create order from cart.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrder()
        {
            var order = await _orderService.CreateOrderAsync(UserId);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        /// <summary>
        /// Get user's orders.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetUserOrders()
        {
            return Ok(await _orderService.GetUserOrdersAsync(UserId));
        }

        /// <summary>
        /// Get order by id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponse>> GetOrderById(int id)
        {
            return Ok(await _orderService.GetOrderByIdAsync(id, UserId));
        }

        /// <summary>
        /// Update order status (Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<ActionResult<OrderResponse>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest updateDto)
        {
            return Ok(await _orderService.UpdateOrderStatusAsync(id, updateDto.Status));
        }

        /// <summary>
        /// Search user orders.
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<OrderResponse>>> SearchUserOrders([FromQuery] OrderParams orderParams)
        {
            return Ok(await _orderService.SearchOrdersAsync(orderParams, UserId));
        }
    }
}
