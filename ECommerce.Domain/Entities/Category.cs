namespace ECommerce.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string? ImageUrl { get; set; }

        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
