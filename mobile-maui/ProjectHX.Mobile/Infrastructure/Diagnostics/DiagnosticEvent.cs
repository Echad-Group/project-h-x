namespace ProjectHX.Mobile.Infrastructure.Diagnostics;

public sealed class DiagnosticEvent
{
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public string Category { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string Severity { get; set; } = "Info";
    public string Message { get; set; } = string.Empty;
    public string? ContextJson { get; set; }
}
