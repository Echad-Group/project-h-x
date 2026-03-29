using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ProjectHX.Mobile.Infrastructure.Diagnostics;

public sealed class AppDiagnosticsService : IAppDiagnosticsService
{
    private readonly ILogger<AppDiagnosticsService> _logger;
    private readonly SemaphoreSlim _fileLock = new(1, 1);
    private readonly string _diagnosticsFilePath;

    public AppDiagnosticsService(ILogger<AppDiagnosticsService> logger)
    {
        _logger = logger;
        var diagnosticsFolder = Path.Combine(FileSystem.AppDataDirectory, "diagnostics");
        Directory.CreateDirectory(diagnosticsFolder);
        _diagnosticsFilePath = Path.Combine(diagnosticsFolder, "events.jsonl");
    }

    public async Task RecordEventAsync(
        string category,
        string eventName,
        string severity,
        string message,
        object? context = null,
        CancellationToken cancellationToken = default)
    {
        var entry = new DiagnosticEvent
        {
            Category = category,
            EventName = eventName,
            Severity = severity,
            Message = message,
            ContextJson = context == null ? null : JsonSerializer.Serialize(context)
        };

        _logger.LogInformation("Diagnostic {Category}/{EventName} [{Severity}] {Message}", category, eventName, severity, message);

        await _fileLock.WaitAsync(cancellationToken);
        try
        {
            await using var stream = File.Open(_diagnosticsFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            await using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(JsonSerializer.Serialize(entry));
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task<IReadOnlyList<DiagnosticEvent>> GetRecentEventsAsync(int maxCount = 100, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_diagnosticsFilePath))
        {
            return [];
        }

        await _fileLock.WaitAsync(cancellationToken);
        try
        {
            var lines = await File.ReadAllLinesAsync(_diagnosticsFilePath, cancellationToken);
            var events = new List<DiagnosticEvent>(capacity: Math.Min(lines.Length, maxCount));

            foreach (var line in lines.Reverse().Take(maxCount))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                try
                {
                    var entry = JsonSerializer.Deserialize<DiagnosticEvent>(line);
                    if (entry is not null)
                    {
                        events.Add(entry);
                    }
                }
                catch
                {
                    // Ignore malformed lines; keep reading remaining diagnostics.
                }
            }

            return events;
        }
        finally
        {
            _fileLock.Release();
        }
    }
}
