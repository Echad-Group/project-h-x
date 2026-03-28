using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile.Pages;

public partial class ResetPasswordPage : ContentPage
{
    public ResetPasswordPage(ResetPasswordViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
