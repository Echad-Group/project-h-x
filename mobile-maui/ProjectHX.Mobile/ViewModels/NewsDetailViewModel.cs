using CommunityToolkit.Mvvm.ComponentModel;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class NewsDetailViewModel : BaseViewModel
{
    private readonly INewsApiService _newsApiService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasArticle))]
    [NotifyPropertyChangedFor(nameof(HasArticleImages))]
    private NewsArticleDetailModel? article;

    public bool HasArticle => Article != null;
    
    public bool HasArticleImages => Article?.ImageUrls?.Count > 1;

    public NewsDetailViewModel(INewsApiService newsApiService)
    {
        _newsApiService = newsApiService;
    }

    public async Task LoadAsync(string slug)
    {
        if (IsBusy || string.IsNullOrWhiteSpace(slug))
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            Article = await _newsApiService.GetBySlugAsync(slug);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
