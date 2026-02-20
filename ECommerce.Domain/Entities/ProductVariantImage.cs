namespace ECommerce.Domain.Entities
{
    public class ProductVariantImage : BaseEntity
    {
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
    }
}
