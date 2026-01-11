using AutoMapper;
using ECommerce.Application.DTO;
using ECommerce.Application.Interfaces.Services.Products;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductParams productParams)
        {
            var pagedProducts = await _productRepository.SearchProductsAsync(productParams);
            
            var dtos = _mapper.Map<IEnumerable<ProductDto>>(pagedProducts.Items);

            return new PagedResult<ProductDto>
            {
                Items = dtos,
                TotalCount = pagedProducts.TotalCount,
                PageNumber = pagedProducts.PageNumber,
                PageSize = pagedProducts.PageSize
            };
        }

        public async Task<ProductDetailsDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetWithFullDetailsAsync(id);
            if (product == null) return null;
            return _mapper.Map<ProductDetailsDto>(product);
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            if (id != request.Id) return false;

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            // Validate Category
            if (!await _categoryRepository.ExistsAsync(request.CategoryId))
                throw new ArgumentException("Invalid Category ID"); // Or return false/custom result

            // Update basic fields
            _mapper.Map(request, product);
            
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
            return true;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
        {
            // Validate Category
            if (!await _categoryRepository.ExistsAsync(request.CategoryId))
                throw new ArgumentException("Invalid Category ID");

            var product = _mapper.Map<Product>(request);
            
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            // Re-fetch to include any db-generated fields or related data if necessary for DTO
            var createdProduct = await _productRepository.GetWithVariantsAsync(product.Id);
            return _mapper.Map<ProductDto>(createdProduct ?? product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            _productRepository.Delete(product);
            await _productRepository.SaveChangesAsync();
            return true;
        }
    }
}
