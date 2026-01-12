using ECommerce.Application.DTO.Categories;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        // Custom Queries
        Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<PagedResult<CategoryDto>> SearchCategoriesAsync(CategoryParams categoryParams, CancellationToken cancellationToken = default);
        
        Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId, CancellationToken cancellationToken = default);
        
        Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    }
}
