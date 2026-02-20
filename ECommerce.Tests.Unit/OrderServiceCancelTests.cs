using AutoMapper;
using ECommerce.Application.DTO.Orders.Responses;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ECommerce.Tests.Unit
{
    public class OrderServiceCancelTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IProductVariantRepository> _variantRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly OrderService _orderService;

        public OrderServiceCancelTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _orderRepoMock = new Mock<IOrderRepository>();
            _variantRepoMock = new Mock<IProductVariantRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<OrderService>>();

            _unitOfWorkMock.Setup(u => u.Orders).Returns(_orderRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ProductVariants).Returns(_variantRepoMock.Object);

            _orderService = new OrderService(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CancelOrderAsync_ShouldSucceed_WhenOrderIsPending()
        {
            // Arrange
            var userId = "user-1";
            var orderId = 1;
            
            // Using constructor for ProductVariant
            var variant = new ProductVariant("SKU1", "Test Variant", 100, 10) { Id = 1 };

            var address = new ShippingAddress { FullName = "Test" }; // ShippingAddress is a snapshot DTO/Owned type, simple props
            var order = new Order(userId, "ORD-1", address) { Id = orderId };
            
            // We need to add item via method
            order.AddItem(variant, 2);

            _orderRepoMock.Setup(r => r.GetByIdWithDetailsAsync(orderId)).ReturnsAsync(order);
            _variantRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(variant);
            _mapperMock.Setup(m => m.Map<OrderResponse>(It.IsAny<Order>()))
                .Returns(new OrderResponse { Id = orderId, Status = "Cancelled" });

            // Act
            var result = await _orderService.CancelOrderAsync(orderId, userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Cancelled);
            variant.StockQuantity.Should().Be(12); // Restored

            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelOrderAsync_ShouldFail_WhenOrderNotFound()
        {
            // Arrange
            _orderRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync((Order)null!);

            // Act
            var result = await _orderService.CancelOrderAsync(1, "user-1");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.NotFound);
        }

        [Fact]
        public async Task CancelOrderAsync_ShouldFail_WhenOrderDoesNotBelongToUser()
        {
            // Arrange
            var address = new ShippingAddress { FullName = "Test" };
            var order = new Order("other-user", "ORD-1", address) { Id = 1 };
            _orderRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(order);

            // Act
            var result = await _orderService.CancelOrderAsync(1, "user-1");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.Unauthorized);
        }

        [Fact]
        public async Task CancelOrderAsync_ShouldFail_WhenOrderIsAlreadyShipped()
        {
            // Arrange
            var userId = "user-1";
            var address = new ShippingAddress { FullName = "Test" };
            var order = new Order(userId, "ORD-1", address) { Id = 1 };
            
            // Manually update status for test setup (if possible via method)
            order.UpdateStatus(OrderStatus.Processing);
            order.UpdateStatus(OrderStatus.Shipped);
            
            _orderRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(order);

            // Act
            var result = await _orderService.CancelOrderAsync(1, userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.InvalidStatusTransition);
        }
    }
}
