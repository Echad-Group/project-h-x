using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices;
using ProjectHX.Mobile.Contexts;
using ProjectHX.Mobile.Models.Profile;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class ProfileAccountViewModel : ProfileSectionViewModelBase
{
    private readonly IUserProfileApiService _userProfileApiService;
    private readonly IAuthApiService _authApiService;
    private readonly IAppNavigator _appNavigator;
    private readonly ISessionService _sessionService;
    private readonly IPushApiService _pushApiService;
    private readonly Func<Task> _refreshProfileAsync;
    private readonly AppStorageContext _appStorageContext;

    [ObservableProperty]
    private string newEmail = string.Empty;

    [ObservableProperty]
    private string emailPassword = string.Empty;

    [ObservableProperty]
    private string currentPassword = string.Empty;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string deletePassword = string.Empty;

    [ObservableProperty]
    private bool isPushSubscribed;

    [ObservableProperty]
    private string pushStatusSummary = "Push status not loaded.";

    public ProfileAccountViewModel(
        BaseViewModel pageState,
        IUserProfileApiService userProfileApiService,
        IAuthApiService authApiService,
        IAppNavigator appNavigator,
        ISessionService sessionService,
        IPushApiService pushApiService,
        Func<Task> refreshProfileAsync,
        AppStorageContext appStorageContext)
        : base(pageState)
    {
        _userProfileApiService = userProfileApiService;
        _authApiService = authApiService;
        _appNavigator = appNavigator;
        _sessionService = sessionService;
        _pushApiService = pushApiService;
        _refreshProfileAsync = refreshProfileAsync;
        _appStorageContext = appStorageContext;
    }

    public void ApplyProfile(UserProfileModel profile)
    {
        NewEmail = profile.Email;
    }

    public void ApplyPushStatus(PushStatusModel status)
    {
        IsPushSubscribed = status.IsSubscribed;
        PushStatusSummary = status.IsSubscribed
            ? $"Subscribed since {status.SubscriptionDate?.ToLocalTime():d MMM yyyy HH:mm}"
            : "Push notifications are disabled on this device.";
    }

    public void ApplyPushLoadFailure()
    {
        IsPushSubscribed = false;
        PushStatusSummary = "Unable to read push status right now.";
    }

    [RelayCommand]
    private async Task UpdateEmailAsync()
    {
        if (IsPageBusy)
        {
            return;
        }

        ClearMessages();

        if (string.IsNullOrWhiteSpace(NewEmail) || string.IsNullOrWhiteSpace(EmailPassword))
        {
            SetError("New email and current password are required.");
            return;
        }

        if (!TryStartOperation())
        {
            return;
        }

        try
        {
            var message = await _userProfileApiService.UpdateEmailAsync(new UpdateEmailRequest
            {
                NewEmail = NewEmail.Trim(),
                Password = EmailPassword
            });

            EmailPassword = string.Empty;
            await _refreshProfileAsync();
            SetInfo(message);
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
        }
        finally
        {
            FinishOperation();
        }
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        if (IsPageBusy)
        {
            return;
        }

        ClearMessages();

        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            SetError("All password fields are required.");
            return;
        }

        if (!string.Equals(NewPassword, ConfirmPassword, StringComparison.Ordinal))
        {
            SetError("New password and confirmation do not match.");
            return;
        }

        if (!TryStartOperation())
        {
            return;
        }

        try
        {
            var message = await _userProfileApiService.ChangePasswordAsync(new ChangePasswordRequest
            {
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
                ConfirmPassword = ConfirmPassword
            });

            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            SetInfo(message);
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
        }
        finally
        {
            FinishOperation();
        }
    }

    [RelayCommand]
    private async Task EnablePushAsync()
    {
        if (IsPageBusy)
        {
            return;
        }

        ClearMessages();

        if (!TryStartOperation())
        {
            return;
        }

        try
        {
            var message = await _pushApiService.SubscribeAsync(new PushSubscriptionRequestModel
            {
                Endpoint = ResolvePushEndpoint(),
                Keys = new PushKeysModel
                {
                    P256dh = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    Auth = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                }
            });

            await LoadPushStatusAsync();
            SetInfo(message);
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
        }
        finally
        {
            FinishOperation();
        }
    }

    [RelayCommand]
    private async Task DisablePushAsync()
    {
        if (IsPageBusy)
        {
            return;
        }

        ClearMessages();

        if (!TryStartOperation())
        {
            return;
        }

        try
        {
            var message = await _pushApiService.UnsubscribeAsync(ResolvePushEndpoint());
            await LoadPushStatusAsync();
            SetInfo(message);
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
        }
        finally
        {
            FinishOperation();
        }
    }

    [RelayCommand]
    private async Task RefreshPushStatusAsync()
    {
        if (IsPageBusy)
        {
            return;
        }

        ClearMessages();

        if (!TryStartOperation())
        {
            return;
        }

        try
        {
            await LoadPushStatusAsync();
        }
        finally
        {
            FinishOperation();
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (IsPageBusy)
        {
            return;
        }

        ClearMessages();

        if (!TryStartOperation())
        {
            return;
        }

        try
        {
            await _authApiService.LogoutAsync();
        }
        catch
        {
        }
        finally
        {
            await _sessionService.ClearAsync(SessionChangeReason.SignedOut);
            Preferences.Remove(_appStorageContext.UserIsBoarded);
            FinishOperation();
            await _appNavigator.GoToWelcomeAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteAccountAsync()
    {
        if (IsPageBusy)
        {
            return;
        }

        ClearMessages();

        if (string.IsNullOrWhiteSpace(DeletePassword))
        {
            SetError("Password is required to delete the account.");
            return;
        }

        if (!TryStartOperation())
        {
            return;
        }

        try
        {
            var message = await _userProfileApiService.DeleteAccountAsync(DeletePassword);
            DeletePassword = string.Empty;
            await _sessionService.ClearAsync(SessionChangeReason.SignedOut);
            SetInfo(message);
            await _appNavigator.GoToLoginAsync();
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
        }
        finally
        {
            FinishOperation();
        }
    }

    private async Task LoadPushStatusAsync()
    {
        try
        {
            ApplyPushStatus(await _pushApiService.GetStatusAsync());
        }
        catch
        {
            ApplyPushLoadFailure();
        }
    }

    private static string ResolvePushEndpoint()
    {
        return $"maui://device/{DeviceInfo.Platform}/{DeviceInfo.Model}/{DeviceInfo.Name}";
    }
}