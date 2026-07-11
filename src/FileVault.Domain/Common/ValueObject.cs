namespace FileVault.Domain.Common;

/// <summary>
/// Base class for Value Objects.
/// Value Objects have NO identity — they are defined by their values.
/// Two Value Objects with the same values are considered equal.
/// Example: FileSize(100) == FileSize(100) regardless of reference.
/// </summary>
public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType()) return false;
        return ((ValueObject)obj)
            .GetEqualityComponents()
            .SequenceEqual(GetEqualityComponents());
    }

    public override int GetHashCode()
        => GetEqualityComponents()
            .Aggregate(0, (hash, val)
                => HashCode.Combine(hash, val?.GetHashCode() ?? 0));

    public static bool operator ==(ValueObject? a, ValueObject? b)
        => a is null ? b is null : a.Equals(b);

    public static bool operator !=(ValueObject? a, ValueObject? b)
        => !(a == b);
}
