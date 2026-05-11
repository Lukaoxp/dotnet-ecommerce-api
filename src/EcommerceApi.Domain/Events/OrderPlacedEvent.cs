using EcommerceApi.Domain.Common;

namespace EcommerceApi.Domain.Events;

public sealed class OrderPlacedEvent : DomainEvent
{
    public Guid OrderId { get; }
    public Guid TenantId { get; }

    public OrderPlacedEvent(Guid orderId, Guid tenantId)
    {
        OrderId = orderId;
        TenantId = tenantId;
    }
}