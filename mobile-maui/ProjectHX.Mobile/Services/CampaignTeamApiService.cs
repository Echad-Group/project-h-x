using System.Net.Http.Json;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class CampaignTeamApiService : ICampaignTeamApiService
{
    private readonly HttpClient _httpClient;

    public CampaignTeamApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CampaignTeamMemberModel>> GetTeamAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("campaignteam", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<CampaignTeamMemberModel>>(cancellationToken: cancellationToken) ?? [];
    }
}
