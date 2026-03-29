using System.Net.Http.Json;
using System.Text.Json;
using ProjectHX.Mobile.Models.Results;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class ResultsApiService : IResultsApiService
{
    private readonly HttpClient _httpClient;

    public ResultsApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> SubmitAsync(ResultSubmissionRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("results/submit", request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await ReadErrorMessageAsync(response, "Failed to submit result.", cancellationToken));
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return "Result submitted.";
        }

        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.TryGetProperty("message", out var message)
            ? message.GetString() ?? "Result submitted."
            : "Result submitted.";
    }

    private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, string fallback, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return fallback;
        }

        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.TryGetProperty("message", out var message)
            ? message.GetString() ?? fallback
            : fallback;
    }
}
