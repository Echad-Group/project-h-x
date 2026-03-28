using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

[QueryProperty(nameof(Email), "email")]
[QueryProperty(nameof(Token), "token")]
public partial class ResetPasswordViewModel : BaseViewModel
{
    private readonly IAuthApiService _authApiService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string token = string.Empty;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    public ResetPasswordViewModel(IAuthApiService authApiService)
    {
        _authApiService = authApiService;
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Token) || string.IsNullOrWhiteSpace(NewPassword))
        {
            ErrorMessage = "Email, reset token, and new password are required.";
            return;
        }

        if (!string.Equals(NewPassword, ConfirmPassword, StringComparison.Ordinal))
        {
            ErrorMessage = "Passwords do not match.";
            return;
        }

        IsBusy = true;

        try
        {
            InfoMessage = await _authApiService.ResetPasswordAsync(new ResetPasswordRequest
            {
                Email = Email.Trim(),
                Token = Token.Trim(),
                NewPassword = NewPassword
            });

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
    private async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync("//login");
    }
}
