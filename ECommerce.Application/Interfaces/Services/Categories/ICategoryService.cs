using ECommerce.Application.DTO.Categories;
using ECommerce.Application.DTO.Pagination;

namespace ECommerce.Application.Interfaces.Services.Categories
{
    /// <summary>
    /// Interface for category service operations.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Retrieves all categories including their parent categories.
        /// </summary>
        Task<IEnumerable<CategoryDto>> GetAllAsync();

        /// <summary>
        /// Retrieves a paginated list of categories with filtering and sorting.
        /// </summary>
        Task<PagedResult<CategoryDto>> GetCategoriesAsync(CategoryParams categoryParams);

        /// <summary>
        /// Retrieves a specific category by ID.
        /// </summary>
        Task<CategoryDto?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new category.
        /// </summary>
        Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        Task<bool> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a category by ID.
        /// </summary>
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
