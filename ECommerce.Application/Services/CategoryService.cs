using AutoMapper;
using ECommerce.Application.DTO;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Application.Interfaces.Services.Categories;

namespace ECommerce.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
        {
            var category = _mapper.Map<Category>(request);

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> UpdateAsync(UpdateCategoryRequest request)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id);
            if (category == null) return false;

            // Use Mapper to update existing entity
            _mapper.Map(request, category);

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync();

            return true;
        }
    }
}
