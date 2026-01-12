using AutoMapper;
using ECommerce.Application.DTO.Auth;
using ECommerce.Application.DTO.Categories;
using ECommerce.Application.DTO.Products;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Application.Interfaces.Services.Categories;

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

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(
                default,
                c => c.ParentCategory
            );
            
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<PagedResult<CategoryDto>> GetCategoriesAsync(CategoryParams categoryParams)
        {
            return await _unitOfWork.Categories.SearchCategoriesAsync(categoryParams);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(
                id, 
                default,
                c => c.ParentCategory
            );
            
            if (category == null) return null;
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
        {
            var category = _mapper.Map<Category>(request);

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            //  Reload with ParentCategory after save
            var savedCategory = await _unitOfWork.Categories.GetByIdAsync(
                category.Id,
                default,
                c => c.ParentCategory
            );

            return _mapper.Map<CategoryDto>(savedCategory ?? category);
        }

        public async Task<bool> UpdateAsync(UpdateCategoryRequest request)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(
                request.Id,
                default,
                c => c.ParentCategory
            );
            
            if (category == null) return false;

            // Use Mapper to update existing entity
            _mapper.Map(request, category);

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return false;

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
