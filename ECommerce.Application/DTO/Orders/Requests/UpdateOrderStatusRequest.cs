using System.ComponentModel.DataAnnotations;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTO.Orders.Requests
{
    public class UpdateOrderStatusRequest
    {
        [Required(ErrorMessage = "Order status is required")]
        [EnumDataType(typeof(OrderStatus), ErrorMessage = "Invalid order status")]
        public OrderStatus Status { get; set; }
    }
}
