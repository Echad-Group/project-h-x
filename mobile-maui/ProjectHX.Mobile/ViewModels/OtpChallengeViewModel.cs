using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

[QueryProperty(nameof(Email), "email")]
public partial class OtpChallengeViewModel : BaseViewModel
{
    private readonly IAuthApiService _authApiService;
    private readonly ISessionService _sessionService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string code = string.Empty;

    public OtpChallengeViewModel(IAuthApiService authApiService, ISessionService sessionService)
    {
        _authApiService = authApiService;
        _sessionService = sessionService;
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
                Purpose = "Login",
                Code = Code
            });

            if (!string.IsNullOrWhiteSpace(response.Token))
            {
                await _sessionService.SaveTokenAsync(response.Token);
                await Shell.Current.GoToAsync("//main/tasks");
                return;
            }

            ErrorMessage = response.Message ?? "OTP verification did not return a token.";
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
