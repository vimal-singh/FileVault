namespace FileVault.Domain.Common;

/// <summary>
/// Marker interface for domain events.
/// Domain events represent something that happened in the domain.
/// They are raised by Aggregate Roots and dispatched after persistence.
/// </summary>
public interface IDomainEvent { }
