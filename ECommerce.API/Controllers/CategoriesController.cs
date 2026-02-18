using ECommerce.Application.DTO.Categories.Requests;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Product categories management.
    /// </summary>
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Get paginated categories.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] CategoryParams categoryParams)
        {
            return Ok(await _categoryService.GetCategoriesAsync(categoryParams));
        }

        /// <summary>
        /// Get all categories.
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _categoryService.GetAllAsync());
        }

        /// <summary>
        /// Get category by id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return category == null ? NotFound() : Ok(category);
        }

        /// <summary>
        /// Create new category (Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = await _categoryService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        /// <summary>
        /// Update category (Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            if (id != request.Id) return BadRequest("ID mismatch");
            var updated = await _categoryService.UpdateAsync(request, cancellationToken);
            return updated ? NoContent() : NotFound();
        }

        /// <summary>
        /// Delete category (Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _categoryService.DeleteAsync(id, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}
