using EcommerceApi.Domain.Entities;
using EcommerceApi.Domain.Common;
using EcommerceApi.Domain.ValueObjects;
using EcommerceApi.Domain.Exceptions;

namespace EcommerceApi.Domain.Tests.Entities;

public class CustomerTests
{
    private static readonly TenantId TenantId = TenantId.From(Guid.NewGuid());

    [Fact]
    public void Create_WithValidData_SetsActiveStatus()
    {
        var customer = Customer.Create(TenantId, "Lucas", Email.From("lucas@email.com"), "11111111111", "+5511999999999");

        Assert.Equal(DeletionStatus.Active, customer.DeletionStatus);
    }

    [Fact]
    public void Anonymize_ReplacesAllPiiWithNeutralValues()
    {
        var customer = Customer.Create(TenantId, "Lucas", Email.From("lucas@email.com"), "11111111111", "+5511999999999");
        customer.RequestDeletion();

        customer.Anonymize();

        Assert.Equal("ANONYMIZED", customer.FullName);
        Assert.Equal(Email.Anonymous, customer.Email);
        Assert.Equal("ANONYMIZED", customer.Cpf);
        Assert.Equal("ANONYMIZED", customer.Phone);
        Assert.Equal(DeletionStatus.Deleted, customer.DeletionStatus);
    }

    [Fact]
    public void Anonymize_WhenNotPendingDeletion_ThrowsDomainException()
    {
        var customer = Customer.Create(TenantId, "Lucas", Email.From("lucas@email.com"), "11111111111", "+5511999999999");

        Assert.Throws<DomainException>(() => customer.Anonymize());
    }
}