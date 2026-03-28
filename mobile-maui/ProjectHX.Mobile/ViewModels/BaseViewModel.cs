using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectHX.Mobile.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? infoMessage;

    public bool IsNotBusy => !IsBusy;

    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasInfoMessage => !string.IsNullOrWhiteSpace(InfoMessage);

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
    }

    partial void OnErrorMessageChanged(string? value)
    {
        OnPropertyChanged(nameof(HasErrorMessage));
    }

    partial void OnInfoMessageChanged(string? value)
    {
        OnPropertyChanged(nameof(HasInfoMessage));
    }
}
