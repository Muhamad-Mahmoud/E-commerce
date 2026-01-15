using ECommerce.Application.DTO.Categories.Requests;
using ECommerce.Application.DTO.Categories.Responses;
using ECommerce.Application.DTO.Pagination;

namespace ECommerce.Application.Interfaces.Services
{
    /// <summary>
    /// Interface for category service operations.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Retrieves all categories including their parent categories.
        /// </summary>
        Task<IEnumerable<CategoryResponse>> GetAllAsync();

        /// <summary>
        /// Retrieves a paginated list of categories with filtering and sorting.
        /// </summary>
        Task<PagedResult<CategoryResponse>> GetCategoriesAsync(CategoryParams categoryParams);

        /// <summary>
        /// Retrieves a specific category by ID.
        /// </summary>
        Task<CategoryResponse?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new category.
        /// </summary>
        Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken);

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
