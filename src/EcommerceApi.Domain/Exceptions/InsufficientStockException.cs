namespace EcommerceApi.Domain.Exceptions;

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(Guid productId, int currentStock, int requested) : base($"Insufficient stock for product {productId}. Current: {currentStock}, Requested: {requested}.") { }
}
