using System.Net.Http.Json;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class CampaignTeamApiService : ICampaignTeamApiService
{
    private readonly HttpClient _httpClient;
    private readonly IApiBaseUrlProvider _apiBaseUrlProvider;

    public CampaignTeamApiService(HttpClient httpClient, IApiBaseUrlProvider apiBaseUrlProvider)
    {
        _httpClient = httpClient;
        _apiBaseUrlProvider = apiBaseUrlProvider;
    }

    public async Task<List<CampaignTeamMemberModel>> GetTeamAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("campaignteam", cancellationToken);
        response.EnsureSuccessStatusCode();

        var members = await response.Content.ReadFromJsonAsync<List<CampaignTeamMemberModel>>(cancellationToken: cancellationToken) ?? [];
        foreach (var member in members)
        {
            member.PhotoUrl = BuildAbsoluteUrl(member.PhotoUrl);
        }

        return members;
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
