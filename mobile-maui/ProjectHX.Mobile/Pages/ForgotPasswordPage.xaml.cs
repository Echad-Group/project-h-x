using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile.Pages;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage(ForgotPasswordViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
