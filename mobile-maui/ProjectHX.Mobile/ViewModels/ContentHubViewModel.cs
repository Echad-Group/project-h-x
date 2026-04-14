using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class ContentHubViewModel : BaseViewModel, IAsyncPageLoadable
{
    private readonly INewsApiService _newsApiService;
    private readonly IEventsApiService _eventsApiService;
    private readonly IIssuesApiService _issuesApiService;
    private readonly ICampaignTeamApiService _campaignTeamApiService;
    private readonly IAppNavigator _appNavigator;

    [ObservableProperty]
    private ObservableCollection<NewsArticleListItemModel> featuredNews = [];

    [ObservableProperty]
    private ObservableCollection<CampaignEventModel> upcomingEvents = [];

    [ObservableProperty]
    private ObservableCollection<IssueModel> issues = [];

    [ObservableProperty]
    private ObservableCollection<CampaignTeamMemberModel> teamMembers = [];

    public ContentHubViewModel(
        INewsApiService newsApiService,
        IEventsApiService eventsApiService,
        IIssuesApiService issuesApiService,
        ICampaignTeamApiService campaignTeamApiService,
        IAppNavigator appNavigator)
    {
        _newsApiService = newsApiService;
        _eventsApiService = eventsApiService;
        _issuesApiService = issuesApiService;
        _campaignTeamApiService = campaignTeamApiService;
        _appNavigator = appNavigator;
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var featuredTask = _newsApiService.GetFeaturedNewsAsync(cancellationToken);
            var eventsTask = _eventsApiService.GetEventsAsync(cancellationToken);
            var issuesTask = _issuesApiService.GetIssuesAsync(cancellationToken);
            var teamTask = _campaignTeamApiService.GetTeamAsync(cancellationToken);

            await Task.WhenAll(featuredTask, eventsTask, issuesTask, teamTask);

            FeaturedNews = new ObservableCollection<NewsArticleListItemModel>((await featuredTask).Take(6));
            UpcomingEvents = new ObservableCollection<CampaignEventModel>((await eventsTask).OrderBy(item => item.Date).Take(6));
            Issues = new ObservableCollection<IssueModel>((await issuesTask).Take(6));
            TeamMembers = new ObservableCollection<CampaignTeamMemberModel>((await teamTask).Take(8));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
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
    private async Task OpenNewsAsync(NewsArticleListItemModel article)
    {
        if (article == null || string.IsNullOrWhiteSpace(article.Slug))
        {
            return;
        }

        await _appNavigator.GoToNewsDetailAsync(article.Slug);
    }

    [RelayCommand]
    private async Task OpenEventAsync(CampaignEventModel eventItem)
    {
        if (eventItem == null)
        {
            return;
        }

        await _appNavigator.GoToEventDetailAsync(eventItem.Id);
    }

    [RelayCommand]
    private async Task OpenIssueAsync(IssueModel issue)
    {
        if (issue == null)
        {
            return;
        }

        await _appNavigator.GoToIssueDetailAsync(issue.Id);
    }

    [RelayCommand]
    private async Task GoToAllNewsAsync()
    {
        await _appNavigator.GoToAllNewsAsync();
    }

    [RelayCommand]
    private async Task GoToAllEventsAsync()
    {
        await _appNavigator.GoToAllEventsAsync();
    }

    [RelayCommand]
    private async Task GoToAllIssuesAsync()
    {
        await _appNavigator.GoToAllIssuesAsync();
    }
}
