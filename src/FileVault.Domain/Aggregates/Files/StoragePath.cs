using FileVault.Domain.Common;

namespace FileVault.Domain.Aggregates.Files;

public sealed class StoragePath : ValueObject
{
    public string Value { get; }

    public StoragePath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Storage path cannot be empty.", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
