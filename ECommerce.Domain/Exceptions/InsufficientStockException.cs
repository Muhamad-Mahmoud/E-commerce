namespace ECommerce.Domain.Exceptions
{
    public class InsufficientStockException : Exception
    {
        public string ProductName { get; }
        public int AvailableStock { get; }
        public int RequestedQuantity { get; }

        public InsufficientStockException(string productName, int availableStock, int requestedQuantity)
            : base($"Insufficient stock for product '{productName}'. Available: {availableStock}, Requested: {requestedQuantity}")
        {
            ProductName = productName;
            AvailableStock = availableStock;
            RequestedQuantity = requestedQuantity;
        }
    }
}
