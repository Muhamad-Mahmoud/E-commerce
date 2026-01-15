using ECommerce.Application.DTO.Categories.Responses;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for Category operations.
    /// </summary>
    public interface ICategoryRepository : IRepository<Category>
    {
        /// <summary>
        /// Gets a category by its name.
        /// </summary>
        Task<Category?> GetByNameAsync(string name);

        /// <summary>
        /// Searches categories with filtering and pagination.
        /// </summary>
        Task<PagedResult<CategoryResponse>> SearchCategoriesAsync(CategoryParams categoryParams);

        /// <summary>
        /// Gets all root categories (no parent category).
        /// </summary>
        Task<IEnumerable<Category>> GetRootCategoriesAsync();

        /// <summary>
        /// Gets all sub-categories under a specific parent category.
        /// </summary>
        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId);

        /// <summary>
        /// Checks if a category with the given name exists.
        /// </summary>
        Task<bool> NameExistsAsync(string name);
    }
}
