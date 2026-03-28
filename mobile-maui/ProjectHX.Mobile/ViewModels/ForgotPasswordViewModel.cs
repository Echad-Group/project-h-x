using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public partial class ForgotPasswordViewModel : BaseViewModel
{
    private readonly IAuthApiService _authApiService;

    [ObservableProperty]
    private string email = string.Empty;

    public ForgotPasswordViewModel(IAuthApiService authApiService)
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

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Email is required.";
            return;
        }

        IsBusy = true;

        try
        {
            InfoMessage = await _authApiService.ForgotPasswordAsync(new ForgotPasswordRequest
            {
                Email = Email.Trim()
            });
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
    private async Task GoToResetAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(Pages.ResetPasswordPage)}?email={Uri.EscapeDataString(Email ?? string.Empty)}");
    }

    [RelayCommand]
    private async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync("//login");
    }
}
