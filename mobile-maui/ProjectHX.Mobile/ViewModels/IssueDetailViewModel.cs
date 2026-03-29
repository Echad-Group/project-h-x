using CommunityToolkit.Mvvm.ComponentModel;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class IssueDetailViewModel : BaseViewModel
{
    private readonly IIssuesApiService _issuesApiService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasIssue))]
    private IssueModel? issue;

    public bool HasIssue => Issue != null;

    public IssueDetailViewModel(IIssuesApiService issuesApiService)
    {
        _issuesApiService = issuesApiService;
    }

    public async Task LoadByIdAsync(int id)
    {
        if (IsBusy || id <= 0)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            Issue = await _issuesApiService.GetIssueByIdAsync(id);
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

    public async Task LoadBySlugAsync(string slug)
    {
        if (IsBusy || string.IsNullOrWhiteSpace(slug))
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            Issue = await _issuesApiService.GetIssueBySlugAsync(slug);
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
