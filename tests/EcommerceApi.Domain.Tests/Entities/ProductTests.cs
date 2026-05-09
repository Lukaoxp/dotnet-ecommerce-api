using EcommerceApi.Domain.Entities;
using EcommerceApi.Domain.Events;
using EcommerceApi.Domain.Exceptions;
using EcommerceApi.Domain.ValueObjects;
using EcommerceApi.Domain.Common;

namespace EcommerceApi.Domain.Tests.Entities;

public class ProductTests
{
    private static readonly Guid TenantId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_RaisesProductCreatedEvent()
    {
        var product = Product.Create(TenantId, "Notebook", "NB-001", Money.Of(5000m, "BRL"), stock: 10);

        var evt = Assert.Single(product.DomainEvents);
        Assert.IsType<ProductCreatedEvent>(evt);
    }

    [Fact]
    public void Create_WithValidData_SetsActiveStatus()
    {
        var product = Product.Create(TenantId, "Notebook", "NB-001", Money.Of(5000m, "BRL"), stock: 10);
        Assert.Equal(DeletionStatus.Active, product.DeletionStatus);
    }

    [Fact]
    public void AdjustStock_WithValidQuantity_UpdatesStock()
    {
        var product = Product.Create(TenantId, "Notebook", "NB-001", Money.Of(5000m, "BRL"), stock: 10);

        product.AdjustStock(-3);

        Assert.Equal(7, product.StockQuantity);
    }

    [Fact]
    public void AdjustStock_BelowZero_ThrowsInsufficientStockException()
    {
        var product = Product.Create(TenantId, "Notebook", "NB-001", Money.Of(5000m, "BRL"), stock: 5);
        Assert.Throws<InsufficientStockException>(() => product.AdjustStock(-10));
    }

    [Fact]
    public void RequestDeletion_SetsStatusPendingDeletion()
    {
        var product = Product.Create(TenantId, "Notebook", "NB-001", Money.Of(5000m, "BRL"), stock: 10);

        product.RequestDeletion();

        Assert.Equal(DeletionStatus.PendingDeletion, product.DeletionStatus);
        Assert.NotNull(product.DeletionScheduledFor);
    }

    [Fact]
    public void CancelDeletion_WhenPending_RestoresActiveStatus()
    {
        var product = Product.Create(TenantId, "Notebook", "NB-001", Money.Of(5000m, "BRL"), stock: 10);
        product.RequestDeletion();
        product.CancelDeletion();

        Assert.Equal(DeletionStatus.Active, product.DeletionStatus);
        Assert.Null(product.DeletionScheduledFor);
    }

    [Fact]
    public void CancelDeletion_WhenNotPending_ThrowsDomainException()
    {
        var product = Product.Create(TenantId, "Notebook", "NB-001", Money.Of(5000m, "BRL"), stock: 10);
        Assert.Throws<DomainException>(() => product.CancelDeletion());
    }
}