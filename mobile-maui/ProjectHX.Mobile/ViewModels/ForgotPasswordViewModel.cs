using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public partial class ForgotPasswordViewModel : BaseViewModel
{
    private readonly IAuthApiService _authApiService;
    private readonly IAppNavigator _appNavigator;

    [ObservableProperty]
    private string email = string.Empty;

    public ForgotPasswordViewModel(IAuthApiService authApiService, IAppNavigator appNavigator)
    {
        _authApiService = authApiService;
        _appNavigator = appNavigator;
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
        await _appNavigator.GoToResetPasswordAsync(Email ?? string.Empty);
    }

    [RelayCommand]
    private async Task GoToLoginAsync()
    {
        await _appNavigator.GoToLoginAsync();
    }
}
