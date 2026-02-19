namespace ECommerce.Domain.Exceptions
{
    public class ConcurrencyConflictException : Exception
    {
        public ConcurrencyConflictException() 
            : base("A concurrency conflict occurred. The data has been modified by another user.")
        {
        }

        public ConcurrencyConflictException(string message) : base(message)
        {
        }

        public ConcurrencyConflictException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
