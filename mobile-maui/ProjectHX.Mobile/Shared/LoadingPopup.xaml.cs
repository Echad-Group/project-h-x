
using Mopups.Interfaces;
using Mopups.Pages;
using Mopups.Services;

namespace ProjectHX.Mobile.Shared;
public partial class LoadingPopup : PopupPage
{
	public static LoadingPopup? Instance { get; private set; }
	public LoadingPopup()
	{
		InitializeComponent();
		BindingContext = new
		{
			Text = "Loading..."
		};
		Instance = this;
	}

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

		// Call Dispose to clean up resources
		Instance = null!;
    }
}


public interface ILoadingService
{
	Task<IDisposable> Show();
}

public class LoadingService : ILoadingService, IDisposable
{
	private readonly IPopupNavigation navigation;

	public LoadingService()
	{
		navigation = MopupService.Instance;
	}

	public void Dispose()
	{
		_ = navigation.PopAsync();
	}

	public async Task<IDisposable> Show()
	{
		await navigation.PushAsync(new LoadingPopup(), true);
		return this;
	}
}