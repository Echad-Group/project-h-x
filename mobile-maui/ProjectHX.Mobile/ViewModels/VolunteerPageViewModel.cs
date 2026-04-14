using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Profile;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public partial class VolunteerPageViewModel : BaseViewModel, IAsyncPageLoadable
{
    private readonly IUserProfileApiService _userProfileApiService;
    private readonly IVolunteerApiService _volunteerApiService;

    public ProfileVolunteerViewModel Volunteer { get; }

    public VolunteerPageViewModel(
        IUserProfileApiService userProfileApiService,
        IVolunteerApiService volunteerApiService,
        IAppNavigator appNavigator)
    {
        _userProfileApiService = userProfileApiService;
        _volunteerApiService = volunteerApiService;
        Volunteer = new ProfileVolunteerViewModel(this, volunteerApiService, RefreshVolunteerAsync, appNavigator);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        InfoMessage = null;

        try
        {
            var profileTask = _userProfileApiService.GetProfileAsync(cancellationToken);
            var volunteerStatusTask = _volunteerApiService.GetMyVolunteerStatusAsync(cancellationToken);

            await Task.WhenAll(profileTask, volunteerStatusTask);

            Volunteer.ApplyMemberProfile(await profileTask);
            Volunteer.ApplyStatus(await volunteerStatusTask);
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

    private async Task RefreshVolunteerAsync()
    {
        Volunteer.ApplyStatus(await _volunteerApiService.GetMyVolunteerStatusAsync());
    }
}
