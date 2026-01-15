using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTO.Categories.Requests
{
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Url(ErrorMessage = "Image URL must be a valid URL")]
        public string? ImageUrl { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Parent Category ID must be a valid positive number")]
        public int? ParentCategoryId { get; set; }
    }
}
