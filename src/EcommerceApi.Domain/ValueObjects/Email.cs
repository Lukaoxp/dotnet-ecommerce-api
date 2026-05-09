using System;
using EcommerceApi.Domain.Common;
using EcommerceApi.Domain.Exceptions;

namespace EcommerceApi.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }
    public static readonly Email Anonymous = new("anonymized@deleted.invalid");
    private Email(string value) => Value = value;
    public static Email From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty");

        return new Email(value.Trim().ToLowerInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
