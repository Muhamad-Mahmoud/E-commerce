namespace ECommerce.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; internal set; }
        public virtual Order Order { get; internal set; } = null!;

        public int ProductVariantId { get; internal set; }
        public virtual ProductVariant ProductVariant { get; internal set; } = null!;

        public string ProductName { get; internal set; } = string.Empty;
        public decimal UnitPrice { get; internal set; }
        public int Quantity { get; internal set; }
        public decimal ItemTotal { get; internal set; }

        // EF Core constructor
        internal OrderItem() { }
    }
}
