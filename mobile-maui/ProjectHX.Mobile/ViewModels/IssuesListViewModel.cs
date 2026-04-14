using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class IssuesListViewModel : BaseViewModel
{
    private readonly IIssuesApiService _issuesApiService;
    private readonly IAppNavigator _appNavigator;

    [ObservableProperty]
    private ObservableCollection<IssueModel> issues = [];

    public IssuesListViewModel(
        IIssuesApiService issuesApiService,
        IAppNavigator appNavigator)
    {
        _issuesApiService = issuesApiService;
        _appNavigator = appNavigator;
    }

    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var issues = await _issuesApiService.GetIssuesAsync();
            Issues = new ObservableCollection<IssueModel>(issues);
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
    private async Task OpenIssueAsync(IssueModel issue)
    {
        if (issue == null)
        {
            return;
        }

        await _appNavigator.GoToIssueDetailAsync(issue.Id);
    }
}
