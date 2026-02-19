using AutoMapper;
using ECommerce.Application.DTO.Categories.Requests;
using ECommerce.Application.DTO.Categories.Responses;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Shared;

namespace ECommerce.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<CategoryResponse>>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(
                c => c.ParentCategory
            );

            return Result.Success(_mapper.Map<IEnumerable<CategoryResponse>>(categories));
        }

        public async Task<Result<PagedResult<CategoryResponse>>> GetCategoriesAsync(CategoryParams categoryParams)
        {
            var pagedResult = await _unitOfWork.Categories.SearchCategoriesAsync(categoryParams);
            return Result.Success(pagedResult);
        }

        public async Task<Result<CategoryResponse>> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(
                id,
                c => c.ParentCategory
            );

            if (category == null) return Result.Failure<CategoryResponse>(DomainErrors.Category.NotFound);
            return Result.Success(_mapper.Map<CategoryResponse>(category));
        }

        public async Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = _mapper.Map<Category>(request);

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            //  Reload with ParentCategory after save
            var savedCategory = await _unitOfWork.Categories.GetByIdAsync(
                category.Id,
                c => c.ParentCategory
            );

            return Result.Success(_mapper.Map<CategoryResponse>(savedCategory ?? category));
        }

        public async Task<Result> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(
                request.Id,
                c => c.ParentCategory
            );

            if (category == null) return Result.Failure(DomainErrors.Category.NotFound);

            // Use Mapper to update existing entity
            _mapper.Map(request, category);

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return Result.Failure(DomainErrors.Category.NotFound);

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
