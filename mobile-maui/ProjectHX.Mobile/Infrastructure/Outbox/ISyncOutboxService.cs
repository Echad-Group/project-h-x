namespace ProjectHX.Mobile.Infrastructure.Outbox;

public interface ISyncOutboxService
{
    Task EnqueueAsync(OutboxItem item, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutboxItem>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task ProcessAsync(CancellationToken cancellationToken = default);
}
