using System.Diagnostics.Contracts;
using EcommerceApi.Domain.Common;

namespace EcommerceApi.Domain.Events;

public sealed class ProductCreatedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public Guid TenantId { get; }
    public string Name { get; }
    public string Sku { get; }

    public ProductCreatedEvent(Guid productId, Guid tenantId, string name, string sku)
    {
        ProductId = productId;
        TenantId = tenantId;
        Name = name;
        Sku = sku;
    }
}