using ECommerce.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.DTO.Products.Requests;
using ECommerce.Application.DTO.Products.Responses;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services
{
    /// <summary>
    /// Service for managing products.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of products based on search parameters.
        /// </summary>
        public async Task<Result<PagedResult<ProductResponse>>> GetProductsAsync(ProductParams productParams)
        {
            var pagedResult = await _unitOfWork.Products.SearchProductsAsync(productParams);
            return Result.Success(pagedResult);
        }

        /// <summary>
        /// Gets a product by its ID, including full details (category, variants).
        /// </summary>
        public async Task<Result<ProductDetailsResponse>> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetWithFullDetailsAsync(id);
            if (product == null) return Result.Failure<ProductDetailsResponse>(DomainErrors.Product.NotFound);
            
            var mapped = _mapper.Map<ProductDetailsResponse>(product);
            return Result.Success(mapped!);
        }

        /// <summary>
        /// Updates an existing product's information.
        /// </summary>
        public async Task<Result> UpdateProductAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken)
        {
            if (id != request.Id) return Result.Failure(DomainErrors.Product.IdMismatch);

            var product = await _unitOfWork.Products.GetByIdAsync(
                id,
                p => p.Category
            );

            if (product == null) 
            {
                _logger.LogWarning("Product {ProductId} not found for update", id);
                return Result.Failure(DomainErrors.Product.NotFound);
            }

            // Validate Category
            if (!await _unitOfWork.Categories.ExistsAsync(request.CategoryId))
            {
                _logger.LogWarning("Category {CategoryId} not found for product {ProductId} update", request.CategoryId, id);
                return Result.Failure(DomainErrors.Category.NotFound);
            }

            try 
            {
                _mapper.Map(request, product);

                _unitOfWork.Products.Update(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Product {ProductId} updated successfully", id);
                return Result.Success();
            }
            catch (ConcurrencyConflictException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict during product update for ID {ProductId}", id);
                return Result.Failure(DomainErrors.General.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                return Result.Failure(DomainErrors.General.ServerError);
            }
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        public async Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
        {
            // Validate Category
            if (!await _unitOfWork.Categories.ExistsAsync(request.CategoryId))
            {
                _logger.LogWarning("Category {CategoryId} not found for product creation", request.CategoryId);
                return Result.Failure<ProductResponse>(DomainErrors.Category.NotFound);
            }

            try 
            {
                var product = _mapper.Map<Product>(request);

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("New product {ProductId} created: {ProductName}", product.Id, product.Name);

                // Reload product to include related data 
                var createdProduct = await _unitOfWork.Products.GetByIdAsync(
                    product.Id,
                    p => p.Category,
                    p => p.Variants
                );

                return Result.Success(_mapper.Map<ProductResponse>(createdProduct ?? product)!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product {ProductName}", request.Name);
                return Result.Failure<ProductResponse>(DomainErrors.General.ServerError);
            }
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        public async Task<Result> DeleteProductAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) 
            {
                _logger.LogWarning("Product {ProductId} not found for deletion", id);
                return Result.Failure(DomainErrors.Product.NotFound);
            }

            try 
            {
                _unitOfWork.Products.Delete(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Product {ProductId} deleted successfully", id);
                return Result.Success();
            }
            catch (ConcurrencyConflictException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict during product deletion for ID {ProductId}", id);
                return Result.Failure(DomainErrors.General.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                return Result.Failure(DomainErrors.General.ServerError);
            }
        }
    }
}
