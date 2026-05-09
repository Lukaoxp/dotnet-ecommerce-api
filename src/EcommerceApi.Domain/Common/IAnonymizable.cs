namespace EcommerceApi.Domain.Common;

public interface IAnonymizable : ISoftDeletable
{
    DateTime? AnonymizationRequestedAt { get; }
    DateTime? AnonymizationScheduledFor { get; }
    DateTime? AnonymizedAt { get; }
    void Anonymize();
}
