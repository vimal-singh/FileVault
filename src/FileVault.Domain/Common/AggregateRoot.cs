namespace FileVault.Domain.Common;

/// <summary>
/// Base class for Aggregate Roots.
/// An Aggregate Root is the entry point to a cluster of domain objects.
/// It ensures consistency boundaries and raises domain events.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyList<IDomainEvent> DomainEvents
        => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
