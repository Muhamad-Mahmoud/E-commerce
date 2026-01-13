using AutoMapper;
using ECommerce.Application.DTO.Auth;
using ECommerce.Application.DTO.Categories;
using ECommerce.Application.DTO.Products;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services.Products;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductParams productParams)
        {
            return await _unitOfWork.Products.SearchProductsAsync(productParams);
        }

        public async Task<ProductDetailsDto?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetWithFullDetailsAsync(id);
            if (product == null) return null;
            return _mapper.Map<ProductDetailsDto>(product);
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken)
        {
            if (id != request.Id) return false;

            var product = await _unitOfWork.Products.GetByIdAsync(
                id,
                p => p.Category
            );
            
            if (product == null) return false;

            // Validate Category
            if (!await _unitOfWork.Categories.ExistsAsync(request.CategoryId))
                throw new ArgumentException("Invalid Category ID");

            _mapper.Map(request, product);
            
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
        {
            // Validate Category
            if (!await _unitOfWork.Categories.ExistsAsync(request.CategoryId))
                throw new ArgumentException("Invalid Category ID");

            var product = _mapper.Map<Product>(request);
            
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload product to include related data 
            var createdProduct = await _unitOfWork.Products.GetByIdAsync(
                product.Id,
                p => p.Category,
                p => p.Variants
            );
            
            return _mapper.Map<ProductDto>(createdProduct ?? product);
        }

        public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return false;

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

