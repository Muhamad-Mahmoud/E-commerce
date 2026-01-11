using ECommerce.Application.DTO;

namespace ECommerce.Application.Interfaces.Services.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CreateCategoryRequest request);
        Task<bool> UpdateAsync(UpdateCategoryRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
