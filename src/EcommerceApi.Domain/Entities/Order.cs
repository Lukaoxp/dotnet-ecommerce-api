using EcommerceApi.Domain.Common;
using EcommerceApi.Domain.Events;
using EcommerceApi.Domain.Exceptions;
using EcommerceApi.Domain.ValueObjects;

namespace EcommerceApi.Domain.Entities;

public sealed class Order : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Guid TenantId { get; private set; }
    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyList<OrderItem> OrderItems => _orderItems;
    public OrderStatus OrderStatus { get; private set; }
    public Money? TotalAmount => _orderItems.Count == 0
        ? null
        : _orderItems.Select(i => i.UnitPrice.Multiply(i.Quantity))
                        .Aggregate((acc, next) => acc.Add(next));
    private Order() { }
    public static Order Create(Guid customerId, Guid tenantId)
    {
        return new Order()
        {
            CustomerId = customerId,
            TenantId = tenantId,
            OrderStatus = OrderStatus.Draft
        };
    }

    public void AddItem(Guid productId, int qty, Money unitPrice)
    {
        OrderItem? item = OrderItems.FirstOrDefault(x => x.ProductId == productId);
        if (item is not null)
            if ((item.Quantity + qty) <= 0) _orderItems.Remove(item);
            else item.AddQty(qty);
        else
            _orderItems.Add(new OrderItem(productId, qty, unitPrice));

        Update();
    }

    public Order Place()
    {
        if (OrderItems.Count == 0)
            throw new DomainException("Order cannot be placed without items");
        OrderStatus = OrderStatus.Placed;
        Update();
        Raise(new OrderPlacedEvent(Id, TenantId));
        return this;
    }

    public void Cancel()
    {
        if (OrderStatus is OrderStatus.Draft or OrderStatus.Placed)
        {
            OrderStatus = OrderStatus.Cancelled;
            Update();
        }
        else
        {
            throw new DomainException($"Order already cancelled");
        }
    }
}

public enum OrderStatus
{
    Draft,
    Placed,
    Cancelled
}