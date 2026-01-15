using AutoMapper;
using ECommerce.Application.DTO.Categories.Requests;
using ECommerce.Application.DTO.Categories.Responses;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

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

        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(
                c => c.ParentCategory
            );

            return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
        }

        public async Task<PagedResult<CategoryResponse>> GetCategoriesAsync(CategoryParams categoryParams)
        {
            return await _unitOfWork.Categories.SearchCategoriesAsync(categoryParams);
        }

        public async Task<CategoryResponse?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(
                id,
                c => c.ParentCategory
            );

            if (category == null) return null;
            return _mapper.Map<CategoryResponse>(category);
        }

        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = _mapper.Map<Category>(request);

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            //  Reload with ParentCategory after save
            var savedCategory = await _unitOfWork.Categories.GetByIdAsync(
                category.Id,
                c => c.ParentCategory
            );

            return _mapper.Map<CategoryResponse>(savedCategory ?? category);
        }

        public async Task<bool> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(
                request.Id,
                c => c.ParentCategory
            );

            if (category == null) return false;

            // Use Mapper to update existing entity
            _mapper.Map(request, category);

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return false;

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
