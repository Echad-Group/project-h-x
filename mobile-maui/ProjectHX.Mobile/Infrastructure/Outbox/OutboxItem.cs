using SQLite;

namespace ProjectHX.Mobile.Infrastructure.Outbox;

public sealed class OutboxItem
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Indexed]
    public string Type { get; set; } = string.Empty;

    public string PayloadJson { get; set; } = string.Empty;

    [Indexed]
    public string Status { get; set; } = "Queued";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    [Indexed]
    public DateTime NextAttemptAtUtc { get; set; } = DateTime.UtcNow;

    public int RetryCount { get; set; }
    public string? LastError { get; set; }
    public string? DeadLetterReason { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}
