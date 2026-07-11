using FileVault.Domain.Common;

namespace FileVault.Domain.Aggregates.Files;

public sealed class ContentType : ValueObject
{
    public string Value { get; }

    public ContentType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Content type cannot be empty.", nameof(value));

        if (!value.Contains('/'))
            throw new ArgumentException("Content type must be in the form type/subtype.", nameof(value));

        Value = value.Trim().ToLowerInvariant();
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
