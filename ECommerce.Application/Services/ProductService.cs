using AutoMapper;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.DTO.Products.Requests;
using ECommerce.Application.DTO.Products.Responses;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Services
{
    /// <summary>
    /// Service for managing products.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

            if (product == null) return Result.Failure(DomainErrors.Product.NotFound);

            // Validate Category
            if (!await _unitOfWork.Categories.ExistsAsync(request.CategoryId))
                return Result.Failure(DomainErrors.Category.NotFound);

            _mapper.Map(request, product);

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        public async Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
        {
            // Validate Category
            if (!await _unitOfWork.Categories.ExistsAsync(request.CategoryId))
                return Result.Failure<ProductResponse>(DomainErrors.Category.NotFound);

            var product = _mapper.Map<Product>(request);

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload product to include related data 
            var createdProduct = await _unitOfWork.Products.GetByIdAsync(
                product.Id,
                p => p.Category,
                p => p.Variants
            );

            return Result.Success(_mapper.Map<ProductResponse>(createdProduct ?? product)!);
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        public async Task<Result> DeleteProductAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return Result.Failure(DomainErrors.Product.NotFound);

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
