namespace EcommerceApi.Domain.Common;

public interface ISoftDeletable
{
    DeletionStatus DeletionStatus { get; }
    public DateTime? DeletionRequestedAt { get; }
    public DateTime? DeletionScheduledFor { get; }
    public DateTime? DeletedAt { get; }
    void RequestDeletion();
    void CancelDeletion();
}

public enum DeletionStatus
{
    Active,
    PendingDeletion,
    Deleted
}
