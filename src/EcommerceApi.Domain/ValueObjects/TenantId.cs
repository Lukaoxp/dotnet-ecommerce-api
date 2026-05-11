using EcommerceApi.Domain.Common;
using EcommerceApi.Domain.Exceptions;

namespace EcommerceApi.Domain.ValueObjects;

public sealed class TenantId : ValueObject
{
    public Guid Value { get; }
    private TenantId(Guid value) => Value = value;
    public static TenantId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("TenantId cannot be empty");
        return new TenantId(value);
    }

    public static TenantId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new DomainException($"'{value}' is not a valid TenantId");
        return From(guid);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}