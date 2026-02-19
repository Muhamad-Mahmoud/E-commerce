using AutoMapper;
using ECommerce.Application.DTO.Orders.Responses;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ECommerce.Tests.Unit
{
    public class OrderServiceUpdateTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IShoppingCartRepository> _cartRepoMock;
        private readonly Mock<IProductVariantRepository> _variantRepoMock;
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly OrderService _orderService;

        public OrderServiceUpdateTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cartRepoMock = new Mock<IShoppingCartRepository>();
            _variantRepoMock = new Mock<IProductVariantRepository>();
            _orderRepoMock = new Mock<IOrderRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<OrderService>>();

            _unitOfWorkMock.Setup(u => u.ShoppingCarts).Returns(_cartRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ProductVariants).Returns(_variantRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Orders).Returns(_orderRepoMock.Object);

            _orderService = new OrderService(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        /// <summary>
        /// Tests that UpdateOrderStatusAsync successfully updates the order status when the transition is valid (e.g., Pending -> Processing).
        /// </summary>
        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldUpdateStatus_WhenTransitionIsValid()
        {
            // Arrange
            var orderId = 1;
            var order = new Order { Id = orderId, Status = OrderStatus.Pending };
            _orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
            _mapperMock.Setup(m => m.Map<OrderResponse>(order)).Returns(new OrderResponse { Id = orderId, Status = "Processing" });

            // Act
            var result = await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Processing);

            // Assert
            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Processing);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that UpdateOrderStatusAsync returns an error and does NOT update status when the transition is invalid (e.g., Pending -> Delivered directly).
        /// </summary>
        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldReturnError_WhenTransitionIsInvalid()
        {
            // Arrange
            var orderId = 1;
            var order = new Order { Id = orderId, Status = OrderStatus.Pending };
            _orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

            // Act - Pending to Delivered is invalid (must go through Processing/Shipped)
            var result = await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Delivered);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.InvalidStatusTransition);
            order.Status.Should().Be(OrderStatus.Pending); // Status valid should not change
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Tests that UpdateOrderStatusAsync returns a NotFound error when trying to update a non-existent order.
        /// </summary>
        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldReturnError_WhenOrderNotFound()
        {
            // Arrange
            _orderRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Order?)null);

            // Act
            var result = await _orderService.UpdateOrderStatusAsync(999, OrderStatus.Shipped);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.NotFound);
        }
    }
}
