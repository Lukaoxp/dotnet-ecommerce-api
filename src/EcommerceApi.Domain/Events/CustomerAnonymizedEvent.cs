using EcommerceApi.Domain.Common;

namespace EcommerceApi.Domain.Events;

public sealed class CustomerAnonymizedEvent : DomainEvent
{
    public Guid CustomerId { get; }
    public Guid TenantId { get; }

    public CustomerAnonymizedEvent(Guid customerId, Guid tenantId)
    {
        CustomerId = customerId;
        TenantId = tenantId;
    }
}