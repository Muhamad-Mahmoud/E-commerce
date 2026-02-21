using ECommerce.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ECommerce.Application.DTO.Categories.Requests;
using ECommerce.Application.DTO.Categories.Responses;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services
{
    /// <summary>
    /// Service for managing categories.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class.
        /// </summary>
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets all categories without pagination.
        /// </summary>
        public async Task<Result<IEnumerable<CategoryResponse>>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(
                c => c.ParentCategory
            );

            var mapped = _mapper.Map<IEnumerable<CategoryResponse>>(categories) ?? Enumerable.Empty<CategoryResponse>();
            return Result.Success(mapped);
        }

        /// <summary>
        /// Gets a paginated list of categories based on search parameters.
        /// </summary>
        public async Task<Result<PagedResult<CategoryResponse>>> GetCategoriesAsync(CategoryParams categoryParams)
        {
            var pagedResult = await _unitOfWork.Categories.SearchCategoriesAsync(categoryParams);
            return Result.Success(pagedResult);
        }

        /// <summary>
        /// Gets a category by ID.
        /// </summary>
        public async Task<Result<CategoryResponse>> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(
                id,
                c => c.ParentCategory
            );

            if (category == null) return Result.Failure<CategoryResponse>(DomainErrors.Category.NotFound);
            return Result.Success(_mapper.Map<CategoryResponse>(category)!);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        public async Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            try 
            {
                var category = _mapper.Map<Category>(request);

                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("New category {CategoryId} created: {CategoryName}", category.Id, category.Name);

                //  Reload with ParentCategory after save
                var savedCategory = await _unitOfWork.Categories.GetByIdAsync(
                    category.Id,
                    c => c.ParentCategory
                );

                return Result.Success(_mapper.Map<CategoryResponse>(savedCategory ?? category)!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category {CategoryName}", request.Name);
                return Result.Failure<CategoryResponse>(DomainErrors.General.ServerError);
            }
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        public async Task<Result> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(
                request.Id,
                c => c.ParentCategory
            );

            if (category == null) 
            {
                _logger.LogWarning("Category {CategoryId} not found for update", request.Id);
                return Result.Failure(DomainErrors.Category.NotFound);
            }

            try 
            {
                // Use Mapper to update existing entity
                _mapper.Map(request, category);

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Category {CategoryId} updated successfully", request.Id);
                return Result.Success();
            }
            catch (ConcurrencyConflictException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict during category update for ID {CategoryId}", request.Id);
                return Result.Failure(DomainErrors.General.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId}", request.Id);
                return Result.Failure(DomainErrors.General.ServerError);
            }
        }

        /// <summary>
        /// Deletes a category.
        /// </summary>
        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) 
            {
                _logger.LogWarning("Category {CategoryId} not found for deletion", id);
                return Result.Failure(DomainErrors.Category.NotFound);
            }

            try 
            {
                _unitOfWork.Categories.Delete(category);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Category {CategoryId} deleted successfully", id);
                return Result.Success();
            }
            catch (ConcurrencyConflictException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict during category deletion for ID {CategoryId}", id);
                return Result.Failure(DomainErrors.General.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {CategoryId}", id);
                return Result.Failure(DomainErrors.General.ServerError);
            }
        }
    }
}
