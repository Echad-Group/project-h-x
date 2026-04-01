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
		await Shell.Current.GoToAsync(nameof(LoginPage));
	}
	private async void OnSignupClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync($"{nameof(RegisterPage)}");
	}

	protected override void OnAppearing()
	{
		NativeCodeHelper.SetStyleColorDarkContent("#F2F2F2");
		base.OnAppearing();
	}
}
