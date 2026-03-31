using Mopups.Interfaces;
using Mopups.Pages;
using System.Windows.Input;

namespace ProjectHX.Mobile.Shared;

public partial class SharedPopup : PopupPage
{

	TaskCompletionSource<bool>? _taskCompletionSource;
	public Task<bool> PopupDismissedTask => _taskCompletionSource!.Task;

	public static bool ReturnValue { get; set; }

	public static SharedPopup? Instance { get; private set; }
	public SharedPopup()
	{
		InitializeComponent();

		// PopupContent.Content = content;
		Instance = this;
		// Set the commands
		var vs = new VerticalStackLayout()
		{
			Spacing = 10,
		};
		vs.Add(new Label { Text = "TaxiFinder", FontSize = 24, FontAttributes = FontAttributes.Bold });
		vs.Add(new Label { Text = "TaxiFinder is working...", FontSize = 20 });

		BindingContext = new
		{
			Content = vs,
			OkText = "OK",
			CancelText = "Cancel",
			OkCommand = OkCommandDef,
			CancelCommand = CancelCommandDef,
			OkVisible = true,
			CancelVisible = false,
		};
	}

	public ICommand OkCommandDef => new Command(async () =>
	{
		ReturnValue = true;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        await Application.Current.Windows[0].Page.Handler.MauiContext.Services.GetService<IPopupNavigation>()!.PopAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    });
	public ICommand CancelCommandDef => new Command(async () =>
	{
		ReturnValue = false;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        await Application.Current.Windows[0].Page.Handler.MauiContext.Services.GetService<IPopupNavigation>()!.PopAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    });

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_taskCompletionSource = new TaskCompletionSource<bool>();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		_taskCompletionSource!.SetResult(ReturnValue);
	}
}
