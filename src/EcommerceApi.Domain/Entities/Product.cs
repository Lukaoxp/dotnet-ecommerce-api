using EcommerceApi.Domain.Common;
using EcommerceApi.Domain.Events;
using EcommerceApi.Domain.Exceptions;
using EcommerceApi.Domain.ValueObjects;

namespace EcommerceApi.Domain.Entities;

public sealed class Product : AggregateRoot, ISoftDeletable
{
    public string Name { get; private set; } = string.Empty;
    public string Sku { get; private set; } = string.Empty;
    public Money Price { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public DeletionStatus DeletionStatus { get; private set; }
    public DateTime? DeletionRequestedAt { get; private set; }
    public DateTime? DeletionScheduledFor { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private Product() { }

    public static Product Create(TenantId tenantId, string name, string sku, Money price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty");
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("Product SKU cannot be empty");
        if (stock < 0)
            throw new DomainException("Initial stock cannot be negative");

        var product = new Product
        {
            TenantId = tenantId,
            Name = name,
            Sku = sku,
            Price = price,
            StockQuantity = stock,
            DeletionStatus = DeletionStatus.Active
        };

        product.Raise(new ProductCreatedEvent(product.Id, tenantId.Value, name, sku));
        return product;
    }

    public void AdjustStock(int quantity)
    {
        if (StockQuantity + quantity < 0)
            throw new InsufficientStockException(Id, StockQuantity, Math.Abs(quantity));

        StockQuantity += quantity;
        Update();
    }

    public void UpdatePrice(Money newPrice)
    {
        Price = newPrice;
        Update();
    }

    public void RequestDeletion()
    {
        if (DeletionStatus == DeletionStatus.PendingDeletion)
            throw new DomainException("Product is already pending deletion.");

        DeletionStatus = DeletionStatus.PendingDeletion;
        DeletionRequestedAt = DateTime.UtcNow;
        DeletionScheduledFor = DateTime.UtcNow.AddHours(24);
        Update();
    }

    public void CancelDeletion()
    {
        if (DeletionStatus != DeletionStatus.PendingDeletion)
            throw new DomainException("Cannot cancel deletion: product is not pending deletion.");

        DeletionStatus = DeletionStatus.Active;
        DeletionRequestedAt = null;
        DeletionScheduledFor = null;
        Update();
    }
}
