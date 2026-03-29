using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthApiService _authApiService;
    private readonly IAppNavigator _appNavigator;
    private readonly ISessionService _sessionService;
    private readonly IAuthFlowStateService _authFlowStateService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public LoginViewModel(IAuthApiService authApiService, IAppNavigator appNavigator, ISessionService sessionService, IAuthFlowStateService authFlowStateService)
    {
        _authApiService = authApiService;
        _appNavigator = appNavigator;
        _sessionService = sessionService;
        _authFlowStateService = authFlowStateService;
    }

    [RelayCommand]
    private async Task SignInAsync()
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
            var response = await _authApiService.LoginAsync(new LoginRequest
            {
                Email = Email,
                Password = Password
            });

            if (response.OtpRequired)
            {
                _authFlowStateService.SetPendingLogin(Email, Password);
                await _appNavigator.GoToOtpChallengeAsync(Email, "Login");
                return;
            }

            if (!string.IsNullOrWhiteSpace(response.Token))
            {
                await _sessionService.SaveTokenAsync(response.Token);
                _authFlowStateService.ClearPendingLogin();
                await _appNavigator.GoToMainAsync();
                return;
            }

            ErrorMessage = response.Message ?? "Authentication did not return a token.";
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
    private async Task GoToRegisterAsync()
    {
        await _appNavigator.GoToRegisterAsync();
    }

    [RelayCommand]
    private async Task GoToForgotPasswordAsync()
    {
        await _appNavigator.GoToForgotPasswordAsync();
    }
}
