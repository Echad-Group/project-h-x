using System.Net.Http.Json;
using System.Text.Json;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class EventsApiService : IEventsApiService
{
    private readonly HttpClient _httpClient;
    private readonly IApiBaseUrlProvider _apiBaseUrlProvider;

    public EventsApiService(HttpClient httpClient, IApiBaseUrlProvider apiBaseUrlProvider)
    {
        _httpClient = httpClient;
        _apiBaseUrlProvider = apiBaseUrlProvider;
    }

    public async Task<List<CampaignEventModel>> GetEventsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("events", cancellationToken);
        response.EnsureSuccessStatusCode();

        var events = await response.Content.ReadFromJsonAsync<List<CampaignEventModel>>(cancellationToken: cancellationToken) ?? [];
        return events.Select(NormalizeEvent).ToList();
    }

    public async Task<CampaignEventModel> GetEventByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"events/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var eventItem = await response.Content.ReadFromJsonAsync<CampaignEventModel>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Event detail response was empty.");

        return NormalizeEvent(eventItem);
    }

    public async Task<CampaignEventModel> GetEventBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"events/slug/{Uri.EscapeDataString(slug)}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var eventItem = await response.Content.ReadFromJsonAsync<CampaignEventModel>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Event detail response was empty.");

        return NormalizeEvent(eventItem);
    }

    public async Task<string> SubmitRsvpAsync(EventRsvpRequestModel request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("eventrsvps", request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(ReadMessage(content) ?? "Failed to submit RSVP.");
        }

        return ReadMessage(content) ?? "RSVP submitted successfully.";
    }

    private static string? ReadMessage(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return null;
        using var doc = JsonDocument.Parse(content);
        if (doc.RootElement.TryGetProperty("message", out var msg)) return msg.GetString();
        return null;
    }

    private CampaignEventModel NormalizeEvent(CampaignEventModel eventItem)
    {
        eventItem.ImageUrl = BuildAbsoluteUrl(eventItem.ImageUrl);
        return eventItem;
    }

    private string? BuildAbsoluteUrl(string? relativeOrAbsoluteUrl)
    {
        if (string.IsNullOrWhiteSpace(relativeOrAbsoluteUrl))
        {
            return null;
        }

        if (Uri.TryCreate(relativeOrAbsoluteUrl, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.ToString();
        }

        if (!relativeOrAbsoluteUrl.StartsWith("/", StringComparison.Ordinal))
        {
            return relativeOrAbsoluteUrl;
        }

        var apiBaseUri = _apiBaseUrlProvider.GetBaseUri();
        var siteRoot = new Uri(apiBaseUri.GetLeftPart(UriPartial.Authority));
        return new Uri(siteRoot, relativeOrAbsoluteUrl).ToString();
    }
}
