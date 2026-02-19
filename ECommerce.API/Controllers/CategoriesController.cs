using ECommerce.Application.DTO.Categories.Requests;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Category management endpoints.
    /// </summary>
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryService _categoryService;
        
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Get paginated list of categories.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] CategoryParams categoryParams)
        {
            return HandleResult(await _categoryService.GetCategoriesAsync(categoryParams));
        }

        /// <summary>
        /// Get all categories without pagination.
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await _categoryService.GetAllAsync());
        }

        /// <summary>
        /// Get category by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return HandleResult(await _categoryService.GetByIdAsync(id));
        }

        /// <summary>
        /// Create a new category (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _categoryService.CreateAsync(request, cancellationToken);
            if (result.IsFailure) return HandleResult(result);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Update an existing category (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            if (id != request.Id) return BadRequest("ID mismatch");
            
            var result = await _categoryService.UpdateAsync(request, cancellationToken);
            if (result.IsFailure) return HandleResult(result);

            return NoContent();
        }

        /// <summary>
        /// Delete a category (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _categoryService.DeleteAsync(id, cancellationToken);
            if (result.IsFailure) return HandleResult(result);

            return NoContent();
        }
    }
}
