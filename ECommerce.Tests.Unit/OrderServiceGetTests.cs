using ECommerce.Domain.Exceptions;
using AutoMapper;
using ECommerce.Application.DTO.Orders.Responses;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ECommerce.Tests.Unit
{
    public class OrderServiceGetTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IShoppingCartRepository> _cartRepoMock;
        private readonly Mock<IProductVariantRepository> _variantRepoMock;
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly OrderService _orderService;

        public OrderServiceGetTests()
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
        /// Tests that GetOrderByIdAsync successfully returns the order details when the order exists and belongs to the requesting user.
        /// </summary>
        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenOrderExistsAndBelongsToUser()
        {
            // Arrange
            var userId = "user-1";
            var orderId = 1;
            var order = new Order { Id = orderId, UserId = userId, OrderNumber = "ORD-1" };
            _orderRepoMock.Setup(r => r.GetByIdWithDetailsAsync(orderId)).ReturnsAsync(order);
            _mapperMock.Setup(m => m.Map<OrderResponse>(order)).Returns(new OrderResponse { Id = orderId, OrderNumber = "ORD-1" });

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId, userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.OrderNumber.Should().Be("ORD-1");
        }

        /// <summary>
        /// Tests that GetOrderByIdAsync returns an Unauthorized error when a user tries to access an order belonging to someone else.
        /// </summary>
        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnError_WhenOrderDoesNotBelongToUser()
        {
            // Arrange
            var userId = "user-1";
            var otherUserId = "user-2";
            var orderId = 1;
            var order = new Order { Id = orderId, UserId = otherUserId, OrderNumber = "ORD-1" };
            _orderRepoMock.Setup(r => r.GetByIdWithDetailsAsync(orderId)).ReturnsAsync(order);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId, userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.Unauthorized);
        }

        /// <summary>
        /// Tests that GetOrderByIdAsync returns a NotFound error when the requested order ID does not exist.
        /// </summary>
        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnError_WhenOrderNotFound()
        {
            // Arrange
            _orderRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync((Order?)null);

            // Act
            var result = await _orderService.GetOrderByIdAsync(999, "user-1");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.NotFound);
        }

        /// <summary>
        /// Tests that GetUserOrdersAsync retrieves all orders associated with a specific user.
        /// </summary>
        [Fact]
        public async Task GetUserOrdersAsync_ShouldReturnOrders()
        {
            // Arrange
            var userId = "user-1";
            var orders = new List<Order> { new Order { Id = 1 }, new Order { Id = 2 } };
            _orderRepoMock.Setup(r => r.GetUserOrdersAsync(userId)).ReturnsAsync(orders);
            _mapperMock.Setup(m => m.Map<IEnumerable<OrderResponse>>(orders)).Returns(new List<OrderResponse> { new OrderResponse(), new OrderResponse() });

            // Act
            var result = await _orderService.GetUserOrdersAsync(userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }

        /// <summary>
        /// Tests that SearchOrdersAsync returns a paged result of orders based on search parameters.
        /// </summary>
        [Fact]
        public async Task SearchOrdersAsync_ShouldReturnPagedResult()
        {
            // Arrange
            var paramsDto = new OrderParams { PageNumber = 1, PageSize = 10 };
            var pagedOrders = new PagedResult<Order>(1, 10, 1, new List<Order> { new Order() });

            _orderRepoMock.Setup(r => r.SearchOrdersAsync(paramsDto, null)).ReturnsAsync(pagedOrders);
            _mapperMock.Setup(m => m.Map<List<OrderResponse>>(pagedOrders.Items)).Returns(new List<OrderResponse> { new OrderResponse() });

            // Act
            var result = await _orderService.SearchOrdersAsync(paramsDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
            result.Value.TotalCount.Should().Be(1);
        }
    }
}
