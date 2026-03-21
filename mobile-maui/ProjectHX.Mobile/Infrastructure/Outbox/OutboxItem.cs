namespace ProjectHX.Mobile.Infrastructure.Outbox;

public sealed class OutboxItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime NextAttemptAtUtc { get; set; } = DateTime.UtcNow;
    public int RetryCount { get; set; }
    public string Status { get; set; } = "Queued";
}
