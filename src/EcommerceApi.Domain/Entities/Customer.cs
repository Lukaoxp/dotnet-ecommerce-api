using EcommerceApi.Domain.Common;
using EcommerceApi.Domain.Events;
using EcommerceApi.Domain.Exceptions;
using EcommerceApi.Domain.ValueObjects;

namespace EcommerceApi.Domain.Entities;

public sealed class Customer : AggregateRoot, IAnonymizable
{
    public string FullName { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string Cpf { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public DateTime ConsentedAt { get; private set; }
    public DeletionStatus DeletionStatus { get; private set; }
    public DateTime? DeletionRequestedAt { get; private set; }
    public DateTime? DeletionScheduledFor { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DateTime? AnonymizationRequestedAt { get; private set; }
    public DateTime? AnonymizationScheduledFor { get; private set; }
    public DateTime? AnonymizedAt { get; private set; }

    private Customer() { }

    public static Customer Create(TenantId tenantId, string fullName, Email email, string cpf, string phone)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Customer name cannot be empty");
        if (string.IsNullOrWhiteSpace(cpf))
            throw new DomainException("CPF cannot be empty");

        var customer = new Customer
        {
            TenantId = tenantId,
            FullName = fullName,
            Email = email,
            Cpf = cpf,
            Phone = phone,
            ConsentedAt = DateTime.UtcNow,
            DeletionStatus = DeletionStatus.Active
        };

        customer.Raise(new CustomerRegisteredEvent(customer.Id, tenantId.Value));
        return customer;
    }

    public void RequestDeletion()
    {
        if (DeletionStatus == DeletionStatus.PendingDeletion)
            throw new DomainException("Customer is already pending deletion.");

        DeletionStatus = DeletionStatus.PendingDeletion;
        DeletionRequestedAt = DateTime.UtcNow;
        DeletionScheduledFor = DateTime.UtcNow.AddHours(24);
        AnonymizationRequestedAt = DateTime.UtcNow;
        AnonymizationScheduledFor = DateTime.UtcNow.AddHours(24);
        Update();
    }

    public void CancelDeletion()
    {
        if (DeletionStatus != DeletionStatus.PendingDeletion)
            throw new DomainException("Cannot cancel deletion: customer is not pending deletion.");

        DeletionStatus = DeletionStatus.Active;
        DeletionRequestedAt = null;
        DeletionScheduledFor = null;
        AnonymizationRequestedAt = null;
        AnonymizationScheduledFor = null;
        Update();
    }

    public void Anonymize()
    {
        if (DeletionStatus != DeletionStatus.PendingDeletion)
            throw new DomainException("Cannot anonymize: customer must be pending deletion first.");

        FullName = "ANONYMIZED";
        Email = Email.Anonymous;
        Cpf = "ANONYMIZED";
        Phone = "ANONYMIZED";
        DeletionStatus = DeletionStatus.Deleted;
        AnonymizedAt = DateTime.UtcNow;
        DeletedAt = DateTime.UtcNow;
        Update();

        Raise(new CustomerAnonymizedEvent(Id, TenantId.Value));
    }
}