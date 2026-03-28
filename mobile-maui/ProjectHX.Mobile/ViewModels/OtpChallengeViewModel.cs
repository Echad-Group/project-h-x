using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

[QueryProperty(nameof(Email), "email")]
[QueryProperty(nameof(Purpose), "purpose")]
[QueryProperty(nameof(Code), "code")]
public partial class OtpChallengeViewModel : BaseViewModel
{
    private readonly IAuthApiService _authApiService;
    private readonly ISessionService _sessionService;
    private readonly IAuthFlowStateService _authFlowStateService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string purpose = "Login";

    public OtpChallengeViewModel(IAuthApiService authApiService, ISessionService sessionService, IAuthFlowStateService authFlowStateService)
    {
        _authApiService = authApiService;
        _sessionService = sessionService;
        _authFlowStateService = authFlowStateService;
    }

    [RelayCommand]
    private async Task VerifyAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;
        IsBusy = true;

        try
        {
            var response = await _authApiService.VerifyOtpAsync(new VerifyOtpRequest
            {
                Email = Email,
                Purpose = Purpose,
                Code = Code
            });

            if (string.Equals(Purpose, "Login", StringComparison.OrdinalIgnoreCase))
            {
                var pendingLogin = _authFlowStateService.GetPendingLogin();
                if (pendingLogin is null)
                {
                    ErrorMessage = "OTP verified. Please sign in again.";
                    await Shell.Current.GoToAsync("//login");
                    return;
                }

                var loginResponse = await _authApiService.LoginAsync(new LoginRequest
                {
                    Email = pendingLogin.Value.Email,
                    Password = pendingLogin.Value.Password
                });

                if (string.IsNullOrWhiteSpace(loginResponse.Token))
                {
                    ErrorMessage = loginResponse.Message ?? "Could not complete sign in after OTP verification.";
                    return;
                }

                await _sessionService.SaveTokenAsync(loginResponse.Token);
                _authFlowStateService.ClearPendingLogin();
                await Shell.Current.GoToAsync("//main");
                return;
            }

            InfoMessage = response.Message ?? "OTP verified. You can now sign in.";
            await Shell.Current.GoToAsync("//login");
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
    private async Task ResendOtpAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;
        IsBusy = true;

        try
        {
            var message = await _authApiService.SendOtpAsync(new SendOtpRequest
            {
                Email = Email,
                Purpose = Purpose
            });

            InfoMessage = message;
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
