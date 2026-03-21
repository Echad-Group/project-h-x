using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile.Pages;

public partial class OtpChallengePage : ContentPage
{
    public OtpChallengePage(OtpChallengeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
