namespace ProjectHX.Mobile.Infrastructure.Outbox;

public interface ISyncOutboxService
{
    event EventHandler? StatusChanged;

    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task EnqueueAsync(OutboxItem item, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutboxItem>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task<OutboxStatusSnapshot> GetStatusAsync(CancellationToken cancellationToken = default);
    Task ProcessAsync(CancellationToken cancellationToken = default);
}
