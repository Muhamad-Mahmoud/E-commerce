using ECommerce.Application.DTO.Auth;
using ECommerce.Application.DTO.Categories;
using ECommerce.Application.DTO.Products;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductParams productParams)
        {
            var result = await _productService.GetProductsAsync(productParams);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {            
            var result = await _productService.CreateProductAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
        {
            if (id != request.Id) return BadRequest("ID mismatch");

            var success = await _productService.UpdateProductAsync(id, request, cancellationToken);
            if (!success) return NotFound();
            
            return NoContent();
        }

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

