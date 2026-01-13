using ECommerce.Application.DTO.Categories;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        // Custom Queries
        Task<Category?> GetByNameAsync(string name);
        /// <summary>
        /// Searches categories with filtering and pagination.
        /// </summary>
        Task<PagedResult<CategoryDto>> SearchCategoriesAsync(CategoryParams categoryParams);
        
        Task<IEnumerable<Category>> GetRootCategoriesAsync();
        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId);
        
        Task<bool> NameExistsAsync(string name);
    }
}
