using System.Text.Json;
using Microsoft.Maui.Networking;
using ProjectHX.Mobile.Models.Results;
using ProjectHX.Mobile.Services.Interfaces;
using SQLite;

namespace ProjectHX.Mobile.Infrastructure.Outbox;

public sealed class SyncOutboxService : ISyncOutboxService
{
    private const string StatusQueued = "Queued";
    private const string StatusProcessing = "Processing";
    private const string StatusRetryScheduled = "RetryScheduled";
    private const string StatusCompleted = "Completed";
    private const string StatusDeadLetter = "DeadLetter";

    private readonly IServiceProvider _serviceProvider;
    private readonly SQLiteAsyncConnection _database;
    private readonly SemaphoreSlim _initializeLock = new(1, 1);
    private readonly SemaphoreSlim _processLock = new(1, 1);
    private bool _initialized;
    private DateTime? _lastProcessedAtUtc;
    private string? _lastError;

    public event EventHandler? StatusChanged;

    public SyncOutboxService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "projecthx-mobile-outbox.db3");
        _database = new SQLiteAsyncConnection(dbPath,
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache);

        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_initialized)
        {
            return;
        }

        await _initializeLock.WaitAsync(cancellationToken);
        try
        {
            if (_initialized)
            {
                return;
            }

            await _database.CreateTableAsync<OutboxItem>();
            _initialized = true;
            await NotifyStatusChangedAsync(cancellationToken);
        }
        finally
        {
            _initializeLock.Release();
        }
    }

    public async Task EnqueueAsync(OutboxItem item, CancellationToken cancellationToken = default)
    {
        await InitializeAsync(cancellationToken);

        item.Id = string.IsNullOrWhiteSpace(item.Id) ? Guid.NewGuid().ToString("N") : item.Id;
        item.Status = string.IsNullOrWhiteSpace(item.Status) ? StatusQueued : item.Status;
        item.CreatedAtUtc = item.CreatedAtUtc == default ? DateTime.UtcNow : item.CreatedAtUtc;
        item.UpdatedAtUtc = DateTime.UtcNow;
        item.NextAttemptAtUtc = item.NextAttemptAtUtc == default ? DateTime.UtcNow : item.NextAttemptAtUtc;

        await _database.InsertOrReplaceAsync(item);
        await NotifyStatusChangedAsync(cancellationToken);

        if (HasInternet())
        {
            _ = Task.Run(() => ProcessAsync(CancellationToken.None));
        }
    }

    public async Task<IReadOnlyList<OutboxItem>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        await InitializeAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var items = await _database.Table<OutboxItem>()
            .Where(item => (item.Status == StatusQueued || item.Status == StatusRetryScheduled || item.Status == StatusProcessing)
                && item.NextAttemptAtUtc <= now)
            .OrderBy(item => item.CreatedAtUtc)
            .ToListAsync();

        return items;
    }

    public async Task<OutboxStatusSnapshot> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        await InitializeAsync(cancellationToken);

        var items = await _database.Table<OutboxItem>().ToListAsync();
        return new OutboxStatusSnapshot
        {
            PendingCount = items.Count(item => item.Status == StatusQueued || item.Status == StatusRetryScheduled),
            ProcessingCount = items.Count(item => item.Status == StatusProcessing),
            DeadLetterCount = items.Count(item => item.Status == StatusDeadLetter),
            LastProcessedAtUtc = _lastProcessedAtUtc,
            LastError = _lastError
        };
    }

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        if (!HasInternet())
        {
            return;
        }

        await InitializeAsync(cancellationToken);

        if (!await _processLock.WaitAsync(0, cancellationToken))
        {
            return;
        }

        try
        {
            var pending = await GetPendingAsync(cancellationToken);
            foreach (var item in pending)
            {
                cancellationToken.ThrowIfCancellationRequested();

                item.Status = StatusProcessing;
                item.UpdatedAtUtc = DateTime.UtcNow;
                await _database.UpdateAsync(item);
                await NotifyStatusChangedAsync(cancellationToken);

                try
                {
                    await DispatchAsync(item, cancellationToken);

                    item.Status = StatusCompleted;
                    item.CompletedAtUtc = DateTime.UtcNow;
                    item.LastError = null;
                    item.DeadLetterReason = null;
                    item.UpdatedAtUtc = DateTime.UtcNow;
                    _lastProcessedAtUtc = item.CompletedAtUtc;
                    _lastError = null;
                }
                catch (HttpRequestException ex)
                {
                    ScheduleRetry(item, ex.Message);
                    _lastError = ex.Message;
                }
                catch (InvalidOperationException ex)
                {
                    MoveToDeadLetter(item, ex.Message);
                    _lastError = ex.Message;
                }
                catch (Exception ex)
                {
                    ScheduleRetry(item, ex.Message);
                    _lastError = ex.Message;
                }

                await _database.UpdateAsync(item);
                await NotifyStatusChangedAsync(cancellationToken);
            }
        }
        finally
        {
            _processLock.Release();
        }
    }

    private async Task DispatchAsync(OutboxItem item, CancellationToken cancellationToken)
    {
        switch (item.Type)
        {
            case "task.complete":
            {
                var payload = JsonSerializer.Deserialize<TaskCompletePayload>(item.PayloadJson)
                    ?? throw new InvalidOperationException("Invalid task completion payload.");
                var tasksApi = _serviceProvider.GetRequiredService<ITasksApiService>();
                await tasksApi.CompleteTaskAsync(payload.TaskId, payload.Notes, cancellationToken);
                break;
            }
            case "result.submit":
            {
                var payload = JsonSerializer.Deserialize<ResultSubmissionRequest>(item.PayloadJson)
                    ?? throw new InvalidOperationException("Invalid result submission payload.");
                var resultsApi = _serviceProvider.GetRequiredService<IResultsApiService>();
                await resultsApi.SubmitAsync(payload, cancellationToken);
                break;
            }
            default:
                throw new InvalidOperationException($"Unsupported outbox item type '{item.Type}'.");
        }
    }

    private static void ScheduleRetry(OutboxItem item, string errorMessage)
    {
        item.RetryCount += 1;
        item.Status = StatusRetryScheduled;
        item.LastError = errorMessage;
        item.UpdatedAtUtc = DateTime.UtcNow;

        var backoffMinutes = Math.Min(Math.Pow(2, item.RetryCount), 60);
        item.NextAttemptAtUtc = DateTime.UtcNow.AddMinutes(backoffMinutes);

        if (item.RetryCount >= 5)
        {
            MoveToDeadLetter(item, errorMessage);
        }
    }

    private static void MoveToDeadLetter(OutboxItem item, string reason)
    {
        item.Status = StatusDeadLetter;
        item.DeadLetterReason = reason;
        item.LastError = reason;
        item.UpdatedAtUtc = DateTime.UtcNow;
    }

    private async Task NotifyStatusChangedAsync(CancellationToken cancellationToken)
    {
        await GetStatusAsync(cancellationToken);
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            _ = Task.Run(() => ProcessAsync(CancellationToken.None));
        }
    }

    private static bool HasInternet() => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    private sealed class TaskCompletePayload
    {
        public int TaskId { get; set; }
        public string? Notes { get; set; }
    }
}
