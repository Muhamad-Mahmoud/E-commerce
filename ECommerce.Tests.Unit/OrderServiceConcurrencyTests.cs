using AutoMapper;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ECommerce.Tests.Unit
{
    public class OrderServiceConcurrencyTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IShoppingCartRepository> _cartRepoMock;
        private readonly Mock<IProductVariantRepository> _variantRepoMock;
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly OrderService _orderService;

        public OrderServiceConcurrencyTests()
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
        /// Tests that CreateOrderAsync correctly handles a ConcurrencyConflictException.
        /// It verifies that when a concurrency exception is thrown (e.g., during database commit), 
        /// the service returns a specific failure result (ConcurrencyConflict) and rolls back the transaction.
        /// </summary>
        [Fact]
        public async Task CreateOrderAsync_ShouldReturnConcurrencyConflict_WhenConcurrencyExceptionIsThrown()
        {
            // Arrange
            var userId = "user-123";
            var variant = new ProductVariant
            {
                Id = 1,
                StockQuantity = 10,
                Product = new Product { Name = "Test Product" },
                SKU = "SKU1"
            };

            var cart = new ShoppingCart
            {
                UserId = userId,
                Items = new List<ShoppingCartItem>
                {
                    new ShoppingCartItem { ProductVariantId = 1, Quantity = 1, ProductVariant = variant }
                }
            };

            _unitOfWorkMock.Setup(u => u.ShoppingCarts.GetByUserIdAsync(userId))
                .ReturnsAsync(cart);

            // Simulate the concurrency exception when committing the transaction
            _unitOfWorkMock.Setup(u => u.CommitTransactionAsync())
                .ThrowsAsync(new ConcurrencyConflictException());

            // Act
            var result = await _orderService.CreateOrderAsync(userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.ConcurrencyConflict);

            // Verify Rollback was called
            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }

        /// <summary>
        /// Tests a simulated race condition where two users attempt to place an order simultaneously.
        /// </summary>
        [Fact]
        public async Task CreateOrderAsync_WhenTwoUsersOrderAtSameTime_OneShouldSucceedAndOneShouldFail()
        {
            // Arrange
            var user1 = "user-1";
            var user2 = "user-2";
            var variant = new ProductVariant
            {
                Id = 1,
                StockQuantity = 10,
                Product = new Product { Name = "Parallel Product" },
                SKU = "PARA1",
                Price = 100
            };

            var cart1 = new ShoppingCart { UserId = user1, Items = new List<ShoppingCartItem> { new ShoppingCartItem { ProductVariantId = 1, Quantity = 1, ProductVariant = variant } } };
            var cart2 = new ShoppingCart { UserId = user2, Items = new List<ShoppingCartItem> { new ShoppingCartItem { ProductVariantId = 1, Quantity = 1, ProductVariant = variant } } };

            _unitOfWorkMock.Setup(u => u.ShoppingCarts.GetByUserIdAsync(user1)).ReturnsAsync(cart1);
            _unitOfWorkMock.Setup(u => u.ShoppingCarts.GetByUserIdAsync(user2)).ReturnsAsync(cart2);

            // Simulate the race condition: 
            // The first one to commit succeeds, the second one throws a concurrency exception
            int commitCalls = 0;
            _unitOfWorkMock.Setup(u => u.CommitTransactionAsync())
                .Returns(() =>
                {
                    commitCalls++;
                    if (commitCalls > 1)
                        return Task.FromException(new ConcurrencyConflictException());

                    return Task.CompletedTask;
                });

            // Act: Run both orders in parallel
            var task1 = _orderService.CreateOrderAsync(user1);
            var task2 = _orderService.CreateOrderAsync(user2);

            var results = await Task.WhenAll(task1, task2);

            // Assert
            results.Should().HaveCount(2);
            results.Should().ContainSingle(r => r.IsSuccess);
            results.Should().ContainSingle(r => !r.IsSuccess && r.Error == DomainErrors.Order.ConcurrencyConflict);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Concurrency conflict")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
