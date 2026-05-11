using EcommerceApi.Domain.Entities;
using EcommerceApi.Domain.Events;
using EcommerceApi.Domain.Exceptions;
using EcommerceApi.Domain.ValueObjects;

namespace EcommerceApi.Domain.Tests.Entities;

public class OrderTests
{
    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid CustomerId = Guid.NewGuid();
    private static readonly Guid ProductId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_SetsDraftStatus()
    {
        var order = Order.Create(CustomerId, TenantId);
        Assert.Equal(OrderStatus.Draft, order.OrderStatus);
    }

    [Fact]
    public void AddItem_NewItem_AddsToList()
    {
        var order = Order.Create(CustomerId, TenantId);
        order.AddItem(ProductId, 2, Money.Of(12m, "BRL"));
        Assert.Single(order.OrderItems);
    }


    [Fact]
    public void AddItem_ExistingItemWithValidQty_ChangesQty()
    {
        var order = Order.Create(CustomerId, TenantId);

        order.AddItem(ProductId, 2, Money.Of(12m, "BRL"));
        order.AddItem(ProductId, 2, Money.Of(12m, "BRL"));
        Assert.Equal(4, order.OrderItems[0].Quantity);
    }

    [Fact]
    public void AddItem_ExistingItemWithNegativeQtyResult_RemovesItem()
    {
        var order = Order.Create(CustomerId, TenantId);

        order.AddItem(ProductId, 2, Money.Of(12m, "BRL"));
        order.AddItem(ProductId, -5, Money.Of(12m, "BRL"));
        Assert.Empty(order.OrderItems);
    }

    [Fact]
    public void Place_WithNoItems_RaisesDomainException()
    {
        var order = Order.Create(CustomerId, TenantId);
        Assert.Throws<DomainException>(() => order.Place());
    }

    [Fact]
    public void Place_ValidOrder_RaisesEvent()
    {
        var order = Order.Create(CustomerId, TenantId);
        order.AddItem(ProductId, 2, Money.Of(12m, "BRL"));
        order.Place();
        var evt = Assert.Single(order.DomainEvents);
        Assert.IsType<OrderPlacedEvent>(evt);
    }

    [Fact]
    public void Place_ValidOrder_ChangesStatusToPlaced()
    {
        var order = Order.Create(CustomerId, TenantId);
        order.AddItem(ProductId, 2, Money.Of(12m, "BRL"));
        order.Place();
        Assert.Equal(OrderStatus.Placed, order.OrderStatus);
    }

    [Fact]
    public void Cancel_WithDraftStatus_ChangesStatus()
    {
        var order = Order.Create(CustomerId, TenantId);
        order.Cancel();
        Assert.Equal(OrderStatus.Cancelled, order.OrderStatus);
    }
    [Fact]
    public void Cancel_WithPlacedStatus_ChangesStatus()
    {
        var order = Order.Create(CustomerId, TenantId);
        order.AddItem(ProductId, 2, Money.Of(12m, "BRL"));
        order.Place();
        Assert.Equal(OrderStatus.Placed, order.OrderStatus);
        order.Cancel();
        Assert.Equal(OrderStatus.Cancelled, order.OrderStatus);
    }

    [Fact]
    public void Cancel_AlreadyCancelledStatus_ThrowsException()
    {
        var order = Order.Create(CustomerId, TenantId);
        order.Cancel();
        Assert.Throws<DomainException>(() => order.Cancel());
    }

    [Fact]
    public void TotalAmount_NoItems_ReturnsNull()
    {
        var order = Order.Create(CustomerId, TenantId);
        Assert.Null(order.TotalAmount);
    }

    [Fact]
    public void TotalAmount_WithItems_ReturnsSumOfItemSubtotals()
    {
        var order = Order.Create(CustomerId, TenantId);
        order.AddItem(ProductId, 2, Money.Of(50, "BRL"));
        order.AddItem(Guid.NewGuid(), 3, Money.Of(30, "BRL"));
        Assert.Equal(Money.Of(190, "BRL"), order.TotalAmount);
    }
}