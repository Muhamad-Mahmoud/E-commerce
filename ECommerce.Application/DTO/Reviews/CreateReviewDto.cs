using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTO.Reviews
{
    public class CreateReviewDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
