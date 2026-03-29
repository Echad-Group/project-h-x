using System.Net.Http.Json;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class NewsApiService : INewsApiService
{
    private readonly HttpClient _httpClient;

    public NewsApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<NewsArticleListItemModel>> GetNewsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("news", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<NewsArticleListItemModel>>(cancellationToken: cancellationToken) ?? [];
    }

    public async Task<List<NewsArticleListItemModel>> GetFeaturedNewsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("news/featured", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<NewsArticleListItemModel>>(cancellationToken: cancellationToken) ?? [];
    }

    public async Task<NewsArticleDetailModel> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"news/{Uri.EscapeDataString(slug)}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NewsArticleDetailModel>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("News detail response was empty.");
    }
}
