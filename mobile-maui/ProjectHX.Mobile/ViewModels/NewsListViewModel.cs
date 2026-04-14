using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class NewsListViewModel : BaseViewModel
{
    private readonly INewsApiService _newsApiService;
    private readonly IAppNavigator _appNavigator;
    private const int PageSize = 12;
    private int _currentPage = 1;

    [ObservableProperty]
    private ObservableCollection<NewsArticleListItemModel> articles = [];

    [ObservableProperty]
    private bool hasMoreItems = true;

    public NewsListViewModel(
        INewsApiService newsApiService,
        IAppNavigator appNavigator)
    {
        _newsApiService = newsApiService;
        _appNavigator = appNavigator;
    }

    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        _currentPage = 1;
        HasMoreItems = true;
        Articles.Clear();

        await LoadMoreAsync();
    }

    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        if (IsBusy || !HasMoreItems)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var articles = await _newsApiService.GetArticlesAsync(_currentPage, PageSize);
            
            if (articles == null || articles.Count == 0)
            {
                HasMoreItems = false;
                return;
            }

            if (articles.Count < PageSize)
            {
                HasMoreItems = false;
            }

            foreach (var article in articles)
            {
                Articles.Add(article);
            }

            _currentPage++;
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

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadAsync();
    }

    [RelayCommand]
    private async Task OpenArticleAsync(NewsArticleListItemModel article)
    {
        if (article == null || string.IsNullOrWhiteSpace(article.Slug))
        {
            return;
        }

        await _appNavigator.GoToNewsDetailAsync(article.Slug);
    }
}
