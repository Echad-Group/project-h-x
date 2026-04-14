using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Contexts;
using ProjectHX.Mobile.Models.Profile;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public partial class ProfileViewModel : BaseViewModel, IAsyncPageLoadable
{
    private readonly IUserProfileApiService _userProfileApiService;
    private readonly IVolunteerApiService _volunteerApiService;
    private readonly IPushApiService _pushApiService;
    private readonly IAppNavigator _appNavigator;

    public ProfileIdentityViewModel Identity { get; }

    public ProfileDetailsViewModel Details { get; }

    public ProfileVolunteerViewModel Volunteer { get; }

    public ProfileAccountViewModel Account { get; }

    public ProfileViewModel(
        IUserProfileApiService userProfileApiService,
        IVolunteerApiService volunteerApiService,
        IAuthApiService authApiService,
        IAppNavigator appNavigator,
        ISessionService sessionService,
        IApiBaseUrlProvider apiBaseUrlProvider,
        IPushApiService pushApiService,
        AppStorageContext? appStorageContext = null)
    {
        _userProfileApiService = userProfileApiService;
        _volunteerApiService = volunteerApiService;
        _pushApiService = pushApiService;
        _appNavigator = appNavigator;

        Identity = new ProfileIdentityViewModel(this, userProfileApiService, apiBaseUrlProvider, RefreshProfileAsync);
        Details = new ProfileDetailsViewModel(this, userProfileApiService, RefreshProfileAsync);
        Volunteer = new ProfileVolunteerViewModel(this, volunteerApiService, RefreshVolunteerAsync, appNavigator);
        Account = new ProfileAccountViewModel(this, userProfileApiService, authApiService, appNavigator, sessionService, pushApiService, RefreshProfileAsync, appStorageContext);
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
            await RefreshAllSectionsAsync(cancellationToken);
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

    private async Task RefreshAllSectionsAsync(CancellationToken cancellationToken)
    {
        var profileTask = _userProfileApiService.GetProfileAsync(cancellationToken);
        var volunteerTask = _volunteerApiService.GetMyVolunteerStatusAsync(cancellationToken);
        var pushStatusTask = GetPushStatusAsync(cancellationToken);

        await Task.WhenAll(profileTask, volunteerTask, pushStatusTask);

        var profile = await profileTask;
        Identity.ApplyProfile(profile);
        Details.ApplyProfile(profile);
        Volunteer.ApplyMemberProfile(profile);
        Account.ApplyProfile(profile);

        Volunteer.ApplyStatus(await volunteerTask);

        var pushStatus = await pushStatusTask;
        if (pushStatus is null)
        {
            Account.ApplyPushLoadFailure();
            return;
        }

        Account.ApplyPushStatus(pushStatus);
    }

    private Task RefreshProfileAsync()
        => RefreshProfileAsync(CancellationToken.None);

    private async Task RefreshProfileAsync(CancellationToken cancellationToken)
    {
        var profile = await _userProfileApiService.GetProfileAsync(cancellationToken);
        Identity.ApplyProfile(profile);
        Details.ApplyProfile(profile);
        Volunteer.ApplyMemberProfile(profile);
        Account.ApplyProfile(profile);
    }

    private Task RefreshVolunteerAsync()
        => RefreshVolunteerAsync(CancellationToken.None);

    private async Task RefreshVolunteerAsync(CancellationToken cancellationToken)
    {
        Volunteer.ApplyStatus(await _volunteerApiService.GetMyVolunteerStatusAsync(cancellationToken));
    }

    private Task RefreshPushStatusAsync()
        => RefreshPushStatusAsync(CancellationToken.None);

    private async Task RefreshPushStatusAsync(CancellationToken cancellationToken)
    {
        var pushStatus = await GetPushStatusAsync(cancellationToken);
        if (pushStatus is null)
        {
            Account.ApplyPushLoadFailure();
            return;
        }

        Account.ApplyPushStatus(pushStatus);
    }

    private async Task<PushStatusModel?> GetPushStatusAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _pushApiService.GetStatusAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }

    [RelayCommand]
    private async Task OpenVolunteerHubAsync()
    {
        await _appNavigator.GoToVolunteerHubAsync();
    }
}
