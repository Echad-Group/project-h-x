using System.Net.Http.Json;
using ProjectHX.Mobile.Models.Inbox;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class InboxApiService : IInboxApiService
{
    private readonly HttpClient _httpClient;

    public InboxApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        if (_httpClient.BaseAddress?.Host.Contains("ngrok", StringComparison.OrdinalIgnoreCase) == true &&
            !_httpClient.DefaultRequestHeaders.Contains("ngrok-skip-browser-warning"))
        {
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
        }
    }

    public async Task<List<InboxMessage>> GetInboxAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("messages/inbox", cancellationToken);
        await ApiResponseReader.EnsureSuccessAsync(response, "Failed to load inbox.", cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<List<InboxMessage>>(cancellationToken: cancellationToken);
        return result ?? [];
    }

    public async Task<string> MarkReadAsync(int messageId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"messages/{messageId}/read", null, cancellationToken);
        return await ApiResponseReader.ReadMessageResponseAsync(response, "Read acknowledged.", "Failed to mark message as read.", cancellationToken);
    }
}
