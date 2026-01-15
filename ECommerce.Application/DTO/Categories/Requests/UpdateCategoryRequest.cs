using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTO.Categories.Requests
{
    public class UpdateCategoryRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
