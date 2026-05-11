using System.ComponentModel;
using EcommerceApi.Domain.Exceptions;
using EcommerceApi.Domain.ValueObjects;

namespace EcommerceApi.Domain.Tests.ValueObjects;

public sealed class TenantIdTests
{
    [Fact]
    public void From_ValidGuid_ReturnsTenantId()
    {
        var guid = Guid.NewGuid();
        var tenantId = TenantId.From(guid);
        Assert.Equal(guid, tenantId.Value);
    }

    [Fact]
    public void From_EmptyGuid_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => TenantId.From(Guid.Empty));
        Assert.Contains("empty", ex.Message);
    }

    [Fact]
    public void From_InvalidString_ThroesDomainException()
    {
        Assert.Throws<DomainException>(() => TenantId.From("not-a-guid"));
    }

    [Fact]
    public void Equality_SameGuid_Equal()
    {
        var guid = Guid.NewGuid();
        Assert.Equal(TenantId.From(guid), TenantId.From(guid));
    }

    [Fact]
    public void Equality_DifferentGuid_NotEqual()
    {
        Assert.NotEqual(TenantId.From(Guid.NewGuid()), TenantId.From(Guid.NewGuid()));
    }
}