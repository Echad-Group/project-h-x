using System.Text.Json;

namespace ProjectHX.Mobile.Services;

internal static class ApiResponseReader
{
    public static async Task EnsureSuccessAsync(HttpResponseMessage response, string failureFallback, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await ReadErrorMessageAsync(response, failureFallback, cancellationToken));
        }
    }

    public static async Task<string> ReadMessageResponseAsync(
        HttpResponseMessage response,
        string successFallback,
        string failureFallback,
        CancellationToken cancellationToken)
    {
        await EnsureSuccessAsync(response, failureFallback, cancellationToken);
        return await ReadMessageAsync(response, successFallback, cancellationToken);
    }

    public static async Task<string> ReadMessageAsync(HttpResponseMessage response, string successFallback, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return successFallback;
        }

        using var document = JsonDocument.Parse(content);
        if (document.RootElement.TryGetProperty("message", out var message))
        {
            return message.GetString() ?? successFallback;
        }

        return successFallback;
    }

    public static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, string fallback, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return fallback;
        }

        using var document = JsonDocument.Parse(content);
        if (document.RootElement.TryGetProperty("message", out var message))
        {
            return message.GetString() ?? fallback;
        }

        if (document.RootElement.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array)
        {
            var values = errors.EnumerateArray().Select(item => item.GetString()).Where(item => !string.IsNullOrWhiteSpace(item));
            var combined = string.Join(Environment.NewLine, values!);
            if (!string.IsNullOrWhiteSpace(combined))
            {
                return combined;
            }
        }

        return fallback;
    }
}