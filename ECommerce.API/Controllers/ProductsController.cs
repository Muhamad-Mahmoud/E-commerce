using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.DTO.Products.Requests;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Manages product operations including retrieval, creation, updating, and deletion.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        /// <param name="productService">The product service.</param>
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Retrieves a paginated list of products with optional filtering and sorting.
        /// </summary>
        /// <param name="productParams">Pagination, filtering, and sorting parameters.</param>
        /// <returns>A paginated list of products.</returns>
        /// <response code="200">Returns list of products.</response>
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductParams productParams)
        {
            var result = await _productService.GetProductsAsync(productParams);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific product by its ID.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The product details including variants.</returns>
        /// <response code="200">Returns the product.</response>
        /// <response code="404">Product not found.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        /// <summary>
        /// Creates a new product. Admin only.
        /// </summary>
        /// <param name="request">The product creation details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created product.</returns>
        /// <response code="201">Product created successfully.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        /// <response code="403">Forbidden - User does not have Admin role.</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            var result = await _productService.CreateProductAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing product. Admin only.
        /// </summary>
        /// <param name="id">The product ID to update.</param>
        /// <param name="request">The product update details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>No content response.</returns>
        /// <response code="204">Product updated successfully.</response>
        /// <response code="400">Invalid request data or ID mismatch.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        /// <response code="403">Forbidden - User does not have Admin role.</response>
        /// <response code="404">Product not found.</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
        {
            if (id != request.Id) return BadRequest("ID mismatch");

            var success = await _productService.UpdateProductAsync(id, request, cancellationToken);
            if (!success) return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Deletes a product. Admin only.
        /// </summary>
        /// <param name="id">The product ID to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>No content response.</returns>
        /// <response code="204">Product deleted successfully.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        /// <response code="403">Forbidden - User does not have Admin role.</response>
        /// <response code="404">Product not found.</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            var success = await _productService.DeleteProductAsync(id, cancellationToken);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}

