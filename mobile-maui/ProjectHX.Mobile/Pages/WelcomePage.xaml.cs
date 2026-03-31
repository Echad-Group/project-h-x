using ProjectHX.Mobile.Helpers;

namespace ProjectHX.Mobile.Pages;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
	}
	private async void OnLoginClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//login");
	}
	private async void OnSignupClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//signup");
	}

	protected override void OnAppearing()
	{
		NativeCodeHelper.SetStyleColorDarkContent(AppUIHelpers.BackgroundGreen);
		base.OnAppearing();
	}
}
