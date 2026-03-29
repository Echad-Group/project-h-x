namespace ProjectHX.Mobile.Infrastructure.Outbox;

public sealed class OutboxStatusSnapshot
{
    public int PendingCount { get; set; }
    public int ProcessingCount { get; set; }
    public int DeadLetterCount { get; set; }
    public DateTime? LastProcessedAtUtc { get; set; }
    public string? LastError { get; set; }
}
