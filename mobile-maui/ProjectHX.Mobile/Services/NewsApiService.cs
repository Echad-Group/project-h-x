using System.Net.Http.Json;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class NewsApiService : INewsApiService
{
    private readonly HttpClient _httpClient;
    private readonly IApiBaseUrlProvider _apiBaseUrlProvider;

    public NewsApiService(HttpClient httpClient, IApiBaseUrlProvider apiBaseUrlProvider)
    {
        _httpClient = httpClient;
        _apiBaseUrlProvider = apiBaseUrlProvider;
    }

    public async Task<List<NewsArticleListItemModel>> GetNewsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("news", cancellationToken);
        response.EnsureSuccessStatusCode();

        var articles = await response.Content.ReadFromJsonAsync<List<NewsArticleListItemModel>>(cancellationToken: cancellationToken) ?? [];
        return articles.Select(NormalizeArticle).ToList();
    }

    public async Task<List<NewsArticleListItemModel>> GetFeaturedNewsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("news/featured", cancellationToken);
        response.EnsureSuccessStatusCode();

        var articles = await response.Content.ReadFromJsonAsync<List<NewsArticleListItemModel>>(cancellationToken: cancellationToken) ?? [];
        return articles.Select(NormalizeArticle).ToList();
    }

    public async Task<NewsArticleDetailModel> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"news/{Uri.EscapeDataString(slug)}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var article = await response.Content.ReadFromJsonAsync<NewsArticleDetailModel>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("News detail response was empty.");

        return NormalizeArticle(article);
    }

    private NewsArticleListItemModel NormalizeArticle(NewsArticleListItemModel article)
    {
        article.FeaturedImageUrl = BuildAbsoluteUrl(article.FeaturedImageUrl);
        article.ImageUrls = article.ImageUrls
            .Select(BuildAbsoluteUrl)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Cast<string>()
            .ToList();

        return article;
    }

    private NewsArticleDetailModel NormalizeArticle(NewsArticleDetailModel article)
    {
        article.FeaturedImageUrl = BuildAbsoluteUrl(article.FeaturedImageUrl);
        article.ImageUrls = article.ImageUrls
            .Select(BuildAbsoluteUrl)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Cast<string>()
            .ToList();

        return article;
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
