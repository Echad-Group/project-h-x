namespace ProjectHX.Mobile.Infrastructure.Diagnostics;

public interface IAppDiagnosticsService
{
    Task RecordEventAsync(
        string category,
        string eventName,
        string severity,
        string message,
        object? context = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DiagnosticEvent>> GetRecentEventsAsync(int maxCount = 100, CancellationToken cancellationToken = default);
}
