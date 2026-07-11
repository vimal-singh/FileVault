namespace FileVault.Domain.Common;

/// <summary>
/// Base class for all domain entities.
/// An Entity is defined by its identity (Id), not its attributes.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity() => Id = Guid.NewGuid();
    protected Entity(Guid id) => Id = id;

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity? a, Entity? b)
        => a is null ? b is null : a.Equals(b);

    public static bool operator !=(Entity? a, Entity? b)
        => !(a == b);
}
