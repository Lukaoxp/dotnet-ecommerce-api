using EcommerceApi.Domain.Common;
using EcommerceApi.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace EcommerceApi.Infrastructure.Services;

public sealed class HttpContextTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private TenantId? _cached;
    public HttpContextTenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public TenantId TenantId
    {
        get
        {
            if (_cached is not null) return _cached;
            var header = _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(header))
                throw new InvalidOperationException("X-Tenant-Id header is required");

            _cached = TenantId.From(header);
            return _cached;
        }
    }

    public bool HasTenant => _httpContextAccessor.HttpContext?.Request.Headers.ContainsKey("X-Tenant-Id") == true;
}