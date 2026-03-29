using ProjectHX.Mobile.Models.PublicContent;

namespace ProjectHX.Mobile.Services.Interfaces;

public interface INewsApiService
{
    Task<List<NewsArticleListItemModel>> GetNewsAsync(CancellationToken cancellationToken = default);
    Task<List<NewsArticleListItemModel>> GetFeaturedNewsAsync(CancellationToken cancellationToken = default);
    Task<NewsArticleDetailModel> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
}
