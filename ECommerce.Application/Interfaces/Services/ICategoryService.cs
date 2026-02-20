using ECommerce.Domain.Exceptions;
using ECommerce.Application.DTO.Categories.Requests;
using ECommerce.Application.DTO.Categories.Responses;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<Result<IEnumerable<CategoryResponse>>> GetAllAsync();

        Task<Result<PagedResult<CategoryResponse>>> GetCategoriesAsync(CategoryParams categoryParams);

        Task<Result<CategoryResponse>> GetByIdAsync(int id);

        Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken);

        Task<Result> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken);

        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
