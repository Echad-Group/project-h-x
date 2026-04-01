using ProjectHX.Mobile.Contexts;
using ProjectHX.Mobile.Models.Profile;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IUserProfileApiService _userProfileApiService;
    private readonly IVolunteerApiService _volunteerApiService;
    private readonly IPushApiService _pushApiService;

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
        AppStorageContext appStorageContext)
    {
        _userProfileApiService = userProfileApiService;
        _volunteerApiService = volunteerApiService;
        _pushApiService = pushApiService;

        Identity = new ProfileIdentityViewModel(this, userProfileApiService, apiBaseUrlProvider, RefreshProfileAsync);
        Details = new ProfileDetailsViewModel(this, userProfileApiService, RefreshProfileAsync);
        Volunteer = new ProfileVolunteerViewModel(this, volunteerApiService, RefreshVolunteerAsync);
        Account = new ProfileAccountViewModel(this, userProfileApiService, authApiService, appNavigator, sessionService, pushApiService, RefreshProfileAsync, appStorageContext);
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
            await RefreshAllSectionsAsync();
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

    private async Task RefreshAllSectionsAsync()
    {
        await RefreshProfileAsync();
        await RefreshVolunteerAsync();
        await RefreshPushStatusAsync();
    }

    private async Task RefreshProfileAsync()
    {
        var profile = await _userProfileApiService.GetProfileAsync();
        Identity.ApplyProfile(profile);
        Details.ApplyProfile(profile);
        Account.ApplyProfile(profile);
    }

    private async Task RefreshVolunteerAsync()
    {
        Volunteer.ApplyStatus(await _volunteerApiService.GetMyVolunteerStatusAsync());
    }

    private async Task RefreshPushStatusAsync()
    {
        try
        {
            Account.ApplyPushStatus(await _pushApiService.GetStatusAsync());
        }
        catch
        {
            Account.ApplyPushLoadFailure();
        }
    }
}
