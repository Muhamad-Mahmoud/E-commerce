using ECommerce.Application.DTO.Categories;
using ECommerce.Application.DTO.Pagination;

namespace ECommerce.Application.Interfaces.Services.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        /// <summary>
        /// Retrieves paginated list of categories with filtering options.
        /// </summary>
        Task<PagedResult<CategoryDto>> GetCategoriesAsync(CategoryParams categoryParams);
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
