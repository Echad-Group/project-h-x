using System.Collections.Concurrent;

namespace ProjectHX.Mobile.Infrastructure.Outbox;

public sealed class SyncOutboxService : ISyncOutboxService
{
    private readonly ConcurrentQueue<OutboxItem> _queue = new();

    public Task EnqueueAsync(OutboxItem item, CancellationToken cancellationToken = default)
    {
        _queue.Enqueue(item);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<OutboxItem>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<OutboxItem>>(_queue.ToArray());
    }

    public Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        // Placeholder: connect to API use cases and apply retry policy.
        return Task.CompletedTask;
    }
}
