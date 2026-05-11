using EcommerceApi.Domain.Common;
using EcommerceApi.Domain.ValueObjects;

namespace EcommerceApi.Domain;

public sealed class OrderItem : Entity
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;

    public OrderItem(Guid productId, int quantity, Money unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public void AddQty(int quantity)
    {
        Quantity += quantity;
        Update();
    }
}
