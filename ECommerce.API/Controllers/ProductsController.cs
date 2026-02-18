using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.DTO.Products.Requests;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Product management.
    /// </summary>
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get paginated products.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductParams productParams)
        {
            return Ok(await _productService.GetProductsAsync(productParams));
        }

        /// <summary>
        /// Get product by id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }

        /// <summary>
        /// Create product (Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            var result = await _productService.CreateProductAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update product (Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
        {
            if (id != request.Id) return BadRequest("ID mismatch");
            var success = await _productService.UpdateProductAsync(id, request, cancellationToken);
            return success ? NoContent() : NotFound();
        }

        /// <summary>
        /// Delete product (Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            var success = await _productService.DeleteProductAsync(id, cancellationToken);
            return success ? NoContent() : NotFound();
        }
    }
}
