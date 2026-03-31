using Android.Media;
using ProjectHX.Mobile.Contexts;
using ProjectHX.Mobile.Helpers;
using ProjectHX.Mobile.Services;
using ProjectHX.Mobile.Services.Interfaces;
using System.Globalization;
using TaxiFinder.Mobile.Helpers;
using DeviceInfo = Microsoft.Maui.Devices.DeviceInfo;

namespace ProjectHX.Mobile.Pages;

[QueryProperty(nameof(ShouldRegister), nameof(ShouldRegister))]
public partial class LoadingPage : ContentPage
{
	private bool _shouldRegister = false;
	public bool ShouldRegister
	{
		get => _shouldRegister;
		set => _shouldRegister = value;
	}

	private readonly ISessionService _sessionService;
    private readonly AppStorageContext _appStorageContext;

    public LoadingPage(ISessionService sessionService, AppStorageContext appStorageContext)
    {
        _sessionService = sessionService;
		_appStorageContext = appStorageContext;

        InitializeComponent();
    }

	async Task RegisterDeviceAsync()
    {
        
    }

	protected override async void OnNavigatedTo(NavigatedToEventArgs args)
	{
        await Task.Delay(2000);

        var userBoarded = Preferences.Get(_appStorageContext.UserIsBoarded, false);

		if(!userBoarded)
		{
			await MainThread.InvokeOnMainThreadAsync(async () => await Shell.Current.GoToAsync("//onboarding"));
			return;
        }

        var hasSession = await _sessionService.HasActiveSessionAsync();
		if (hasSession)
		{
            await MainThread.InvokeOnMainThreadAsync(async () => await Shell.Current.GoToAsync("//main"));
			return;
        }

		await MainThread.InvokeOnMainThreadAsync(async () => await Shell.Current.GoToAsync("//welcome"));

        base.OnNavigatedTo(args);
    }

	protected override void OnAppearing()
	{
		PopupHelper.Initialize();
		base.OnAppearing();
	}
}