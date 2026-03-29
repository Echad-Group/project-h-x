using System.Net.Http.Json;
using System.Text.Json;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class PushApiService : IPushApiService
{
    private readonly HttpClient _httpClient;

    public PushApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> SubscribeAsync(PushSubscriptionRequestModel request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("push/subscribe", request, cancellationToken);
        return await ReadMessageResponseAsync(response, "Push subscription updated.", "Failed to update push subscription.", cancellationToken);
    }

    public async Task<string> UnsubscribeAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("push/unsubscribe", new { endpoint }, cancellationToken);
        return await ReadMessageResponseAsync(response, "Push unsubscribed.", "Failed to unsubscribe from push.", cancellationToken);
    }

    public async Task<PushStatusModel> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("push/status", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await ReadErrorMessageAsync(response, "Failed to load push status.", cancellationToken));
        }

        return await response.Content.ReadFromJsonAsync<PushStatusModel>(cancellationToken: cancellationToken)
            ?? new PushStatusModel();
    }

    private static async Task<string> ReadMessageResponseAsync(HttpResponseMessage response, string successFallback, string failureFallback, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await ReadErrorMessageAsync(response, failureFallback, cancellationToken));
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return successFallback;
        }

        using var doc = JsonDocument.Parse(content);
        if (doc.RootElement.TryGetProperty("message", out var message))
        {
            return message.GetString() ?? successFallback;
        }

        return successFallback;
    }

    private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, string fallback, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return fallback;
        }

        using var doc = JsonDocument.Parse(content);
        if (doc.RootElement.TryGetProperty("message", out var message))
        {
            return message.GetString() ?? fallback;
        }

        return fallback;
    }
}
