using EcommerceApi.Domain.ValueObjects;

namespace EcommerceApi.Domain.Common;

public interface ITenantContext
{
    TenantId TenantId { get; }
    bool HasTenant { get; }
}