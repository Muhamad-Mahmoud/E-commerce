using ECommerce.Application.DTO.Categories.Requests;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Manages product categories including retrieval, creation, updating, and deletion.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoriesController"/> class.
        /// </summary>
        /// <param name="categoryService">The category service.</param>
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Retrieves a paginated list of categories with optional filtering and sorting.
        /// </summary>
        /// <param name="categoryParams">Pagination, filtering, and sorting parameters.</param>
        /// <returns>A paginated list of categories.</returns>
        /// <response code="200">Returns list of categories.</response>
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] CategoryParams categoryParams)
        {
            var result = await _categoryService.GetCategoriesAsync(categoryParams);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all categories without pagination.
        /// </summary>
        /// <returns>A list of all categories.</returns>
        /// <response code="200">Returns all categories.</response>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Retrieves a specific category by its ID.
        /// </summary>
        /// <param name="id">The category ID.</param>
        /// <returns>The category details.</returns>
        /// <response code="200">Returns the category.</response>
        /// <response code="404">Category not found.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        /// <summary>
        /// Creates a new category. Admin only.
        /// </summary>
        /// <param name="request">The category creation details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created category.</returns>
        /// <response code="201">Category created successfully.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        /// <response code="403">Forbidden - User does not have Admin role.</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = await _categoryService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        /// <summary>
        /// Updates an existing category. Admin only.
        /// </summary>
        /// <param name="id">The category ID to update.</param>
        /// <param name="request">The category update details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>No content response.</returns>
        /// <response code="204">Category updated successfully.</response>
        /// <response code="400">Invalid request data or ID mismatch.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        /// <response code="403">Forbidden - User does not have Admin role.</response>
        /// <response code="404">Category not found.</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            if (id != request.Id) return BadRequest("ID mismatch");

            var updated = await _categoryService.UpdateAsync(request, cancellationToken);
            if (!updated) return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Deletes a category. Admin only.
        /// </summary>
        /// <param name="id">The category ID to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>No content response.</returns>
        /// <response code="204">Category deleted successfully.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        /// <response code="403">Forbidden - User does not have Admin role.</response>
        /// <response code="404">Category not found.</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _categoryService.DeleteAsync(id, cancellationToken);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
