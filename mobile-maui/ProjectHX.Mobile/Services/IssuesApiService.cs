using System.Net.Http.Json;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class IssuesApiService : IIssuesApiService
{
    private readonly HttpClient _httpClient;

    public IssuesApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<IssueModel>> GetIssuesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("issues", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<IssueModel>>(cancellationToken: cancellationToken) ?? [];
    }

    public async Task<IssueModel> GetIssueByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"issues/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IssueModel>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Issue detail response was empty.");
    }

    public async Task<IssueModel> GetIssueBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"issues/slug/{Uri.EscapeDataString(slug)}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IssueModel>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Issue detail response was empty.");
    }
}
