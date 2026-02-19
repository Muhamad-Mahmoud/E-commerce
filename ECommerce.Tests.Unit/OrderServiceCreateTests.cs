using AutoMapper;
using ECommerce.Application.DTO.Orders.Responses;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ECommerce.Tests.Unit
{
    public class OrderServiceCreateTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IShoppingCartRepository> _cartRepoMock;
        private readonly Mock<IProductVariantRepository> _variantRepoMock;
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly OrderService _orderService;

        public OrderServiceCreateTests()
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
        /// Tests that CreateOrderAsync returns a failure result when the user ID is invalid (null or empty).
        /// </summary>
        [Fact]
        public async Task CreateOrderAsync_ShouldReturnError_WhenUserIdIsInvalid()
        {
            // Act
            var result = await _orderService.CreateOrderAsync("");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.User.IdRequired);
        }

        /// <summary>
        /// Tests that CreateOrderAsync returns a failure result when the user's shopping cart is empty or does not exist.
        /// </summary>
        [Fact]
        public async Task CreateOrderAsync_ShouldReturnError_WhenCartIsEmpty()
        {
            // Arrange
            var userId = "user-1";
            var cart = new ShoppingCart { UserId = userId, Items = new List<ShoppingCartItem>() };
            _cartRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(cart);

            // Act
            var result = await _orderService.CreateOrderAsync(userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.EmptyCart);
        }

        /// <summary>
        /// Tests the happy path for CreateOrderAsync.
        /// Verifies that an order is successfully created, stock is deducted, the cart is cleared, and a transaction is committed.
        /// </summary>
        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrder_WhenCartHasItemsAndStockIsAvailable()
        {
            // Arrange
            var userId = "user-1";
            var variant = new ProductVariant
            {
                Id = 1,
                StockQuantity = 10,
                Product = new Product { Name = "Test Product" },
                SKU = "SKU1",
                Price = 100
            };
            var cart = new ShoppingCart
            {
                UserId = userId,
                Items = new List<ShoppingCartItem>
                {
                    new ShoppingCartItem { ProductVariantId = 1, Quantity = 2, ProductVariant = variant }
                }
            };
            _cartRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(cart);
            _mapperMock.Setup(m => m.Map<OrderResponse>(It.IsAny<Order>()))
                .Returns(new OrderResponse { Id = 1, OrderNumber = "ORD-123", TotalAmount = 200 });

            // Act
            var result = await _orderService.CreateOrderAsync(userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.Orders.AddAsync(It.IsAny<Order>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.ShoppingCarts.Delete(cart), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);

            // Stock should be deducted
            variant.StockQuantity.Should().Be(8);
        }

        /// <summary>
        /// Tests that CreateOrderAsync fails and rolls back the transaction when there is insufficient stock for a requested item.
        /// </summary>
        [Fact]
        public async Task CreateOrderAsync_ShouldReturnError_WhenStockIsInsufficient()
        {
            // Arrange
            var userId = "user-1";
            var variant = new ProductVariant
            {
                Id = 1,
                StockQuantity = 1, // Only 1 in stock
                Product = new Product { Name = "Test Product" },
                SKU = "SKU1",
                Price = 100
            };
            var cart = new ShoppingCart
            {
                UserId = userId,
                Items = new List<ShoppingCartItem>
                {
                    new ShoppingCartItem { ProductVariantId = 1, Quantity = 2, ProductVariant = variant } // Requesting 2
                }
            };
            _cartRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(cart);

            // Act
            var result = await _orderService.CreateOrderAsync(userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.InsufficientStock);

            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }

        /// <summary>
        /// Tests that CreateOrderAsync handles unexpected exceptions by rolling back the transaction and re-throwing the exception.
        /// This ensures database integrity even when unforeseen errors occur.
        /// </summary>
        [Fact]
        public async Task CreateOrderAsync_ShouldRollback_WhenGenericExceptionOccurs()
        {
            // Arrange
            var userId = "user-1";
            var cart = new ShoppingCart
            {
                UserId = userId,
                Items = new List<ShoppingCartItem>
                {
                    new ShoppingCartItem { ProductVariantId = 1, Quantity = 1, ProductVariant = new ProductVariant { Id = 1, StockQuantity = 10, Product = new Product(), Price = 10 } }
                }
            };

            _cartRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(cart);
            _unitOfWorkMock.Setup(u => u.Orders.AddAsync(It.IsAny<Order>())).ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _orderService.CreateOrderAsync(userId);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }
    }
}
