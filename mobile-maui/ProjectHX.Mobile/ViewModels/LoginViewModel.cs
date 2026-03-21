using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthApiService _authApiService;
    private readonly ISessionService _sessionService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public LoginViewModel(IAuthApiService authApiService, ISessionService sessionService)
    {
        _authApiService = authApiService;
        _sessionService = sessionService;
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
                await Shell.Current.GoToAsync($"{nameof(Pages.OtpChallengePage)}?email={Uri.EscapeDataString(Email)}");
                return;
            }

            if (!string.IsNullOrWhiteSpace(response.Token))
            {
                await _sessionService.SaveTokenAsync(response.Token);
                await Shell.Current.GoToAsync("//main/tasks");
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
}
