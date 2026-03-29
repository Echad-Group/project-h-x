using System.Net.Http.Json;
using System.Text.Json;
using ProjectHX.Mobile.Models.Inbox;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class InboxApiService : IInboxApiService
{
    private readonly HttpClient _httpClient;

    public InboxApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<InboxMessage>> GetInboxAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("messages/inbox", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await ReadErrorMessageAsync(response, "Failed to load inbox.", cancellationToken));
        }

        var result = await response.Content.ReadFromJsonAsync<List<InboxMessage>>(cancellationToken: cancellationToken);
        return result ?? [];
    }

    public async Task<string> MarkReadAsync(int messageId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"messages/{messageId}/read", null, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await ReadErrorMessageAsync(response, "Failed to mark message as read.", cancellationToken));
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content)) return "Read acknowledged.";

        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.TryGetProperty("message", out var msg) ? msg.GetString() ?? "Read acknowledged." : "Read acknowledged.";
    }

    private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, string fallback, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content)) return fallback;

        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.TryGetProperty("message", out var msg) ? msg.GetString() ?? fallback : fallback;
    }
}
