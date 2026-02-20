namespace ECommerce.Domain.Entities
{
    public class ShoppingCartItem : BaseEntity
    {
        public int ShoppingCartId { get; internal set; }
        public virtual ShoppingCart ShoppingCart { get; internal set; } = null!;
        public int ProductVariantId { get; internal set; }
        public virtual ProductVariant ProductVariant { get; internal set; } = null!;
        public int Quantity { get; internal set; }
        public decimal UnitPrice { get; internal set; }

        internal ShoppingCartItem() { }

        public ShoppingCartItem(int shoppingCartId, int productVariantId, int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
            
            ShoppingCartId = shoppingCartId;
            ProductVariantId = productVariantId;
            Quantity = quantity;
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0) throw new ArgumentException("Quantity must be positive.");
            Quantity = newQuantity;
        }
    }
}
