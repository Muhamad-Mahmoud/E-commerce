using System;

namespace ECommerce.Application.DTO.Reviews
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? Title { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
