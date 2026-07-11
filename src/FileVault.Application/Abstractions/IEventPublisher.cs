namespace FileVault.Application.Abstractions;

/// <summary>
/// Event publisher abstraction — Application defines the contract.
/// Infrastructure implements it with Kafka.
/// This keeps the Application layer completely unaware of Kafka.
/// </summary>
public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, string topic, CancellationToken ct = default)
        where TEvent : class;
}
