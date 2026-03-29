using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthApiService _authApiService;
    private readonly IAppNavigator _appNavigator;

    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    public RegisterViewModel(IAuthApiService authApiService, IAppNavigator appNavigator)
    {
        _authApiService = authApiService;
        _appNavigator = appNavigator;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "All fields are required.";
            return;
        }

        if (!string.Equals(Password, ConfirmPassword, StringComparison.Ordinal))
        {
            ErrorMessage = "Passwords do not match.";
            return;
        }

        IsBusy = true;

        try
        {
            var response = await _authApiService.RegisterAsync(new RegisterRequest
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password
            });

            if (response.OtpRequired)
            {
                await _appNavigator.GoToOtpChallengeAsync(Email, "Registration");
                return;
            }

            InfoMessage = response.Message ?? "Registration complete. Please sign in.";
            await _appNavigator.GoToLoginAsync();
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
    private async Task GoToLoginAsync()
    {
        await _appNavigator.GoToLoginAsync();
    }
}
