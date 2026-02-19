using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.DTO.Products.Requests;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Product catalog management.
    /// </summary>
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get paginated list of products with optional filtering.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductParams productParams)
        {
            return HandleResult(await _productService.GetProductsAsync(productParams));
        }

        /// <summary>
        /// Get product details by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            return HandleResult(await _productService.GetProductByIdAsync(id));
        }

        /// <summary>
        /// Create a new product (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            var result = await _productService.CreateProductAsync(request, cancellationToken);
            if (result.IsFailure) return HandleResult(result);
            
            return CreatedAtAction(nameof(GetProduct), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Update an existing product (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
        {
            return HandleResult(await _productService.UpdateProductAsync(id, request, cancellationToken));
        }

        /// <summary>
        /// Delete a product (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            return HandleResult(await _productService.DeleteProductAsync(id, cancellationToken));
        }
    }
}
