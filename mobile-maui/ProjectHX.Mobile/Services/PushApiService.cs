using System.Net.Http.Json;
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
        return await ApiResponseReader.ReadMessageResponseAsync(response, "Push subscription updated.", "Failed to update push subscription.", cancellationToken);
    }

    public async Task<string> UnsubscribeAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("push/unsubscribe", new { endpoint }, cancellationToken);
        return await ApiResponseReader.ReadMessageResponseAsync(response, "Push unsubscribed.", "Failed to unsubscribe from push.", cancellationToken);
    }

    public async Task<PushStatusModel> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("push/status", cancellationToken);
        await ApiResponseReader.EnsureSuccessAsync(response, "Failed to load push status.", cancellationToken);

        return await response.Content.ReadFromJsonAsync<PushStatusModel>(cancellationToken: cancellationToken)
            ?? new PushStatusModel();
    }
}
