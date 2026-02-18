namespace ECommerce.Domain.Entities
{
    public class Review : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public string? Title { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsApproved { get; set; } = true; // Default to true for now or as per requirement
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

