using ECommerce.Application.DTO.Categories;
using ECommerce.Application.DTO.Pagination;

namespace ECommerce.Application.Interfaces.Services.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<PagedResult<CategoryDto>> GetCategoriesAsync(CategoryParams categoryParams);
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CreateCategoryRequest request);
        Task<bool> UpdateAsync(UpdateCategoryRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
