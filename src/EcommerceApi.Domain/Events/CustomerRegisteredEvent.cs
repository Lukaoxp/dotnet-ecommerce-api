using EcommerceApi.Domain.Common;

namespace EcommerceApi.Domain.Events;

public sealed class CustomerRegisteredEvent : DomainEvent
{
    public Guid CustomerId { get; }
    public Guid TenantId { get; }

    public CustomerRegisteredEvent(Guid customerId, Guid tenantId)
    {
        CustomerId = customerId;
        TenantId = tenantId;
    }
}