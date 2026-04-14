using ProjectHX.Mobile.Shared;
using Mopups.Interfaces;
using Mopups.Services;
using AppInfo = ProjectHX.Models.Configuration.AppInfo;
using System.Windows.Input;
using TaxiFinder.Mobile.Helpers;

namespace ProjectHX.Mobile.Helpers
{
	public static class PopupHelper
	{
		public static AppInfo? AppInfo = null!;

		public static IPopupNavigation? PopupNavigation = null!;
		public static ILoadingService? Loading = null!;

		public static LoadingPopup LoadingPopup = new();
		
		public static void Initialize()
		{
			PopupNavigation = Application.Current.Windows[0].Page.Handler.MauiContext.Services.GetService<IPopupNavigation>()!;
			Loading = Application.Current.Windows[0].Page.Handler.MauiContext.Services.GetService<ILoadingService>()!;
		}

		public delegate void CallbackDelegate();

		public static object context = null!;

		public static CallbackDelegate? OkCallback { get; set; } = null!;
		public static CallbackDelegate? CancelCallback { get; set; } = null;

		public static ICommand OkCommandDef => new Command(async () =>
		{
			if (OkCallback != null)
			{
				OkCallback();
			}
			SharedPopup.ReturnValue = true;
			await Application.Current.Windows[0].Page.Handler.MauiContext.Services.GetService<IPopupNavigation>()!.PopAsync();
		});
		public static ICommand CancelCommandDef => new Command(async () =>
		{
			if (CancelCallback != null)
			{
				CancelCallback();
			}
			SharedPopup.ReturnValue = false;
			await Application.Current.Windows[0].Page.Handler.MauiContext.Services.GetService<IPopupNavigation>()!.PopAsync();
		});

		 public static async Task<bool> ConstructPopup(string title, string msg,
			string ok = "Ok",
			CallbackDelegate okCallback = null!,
			string cancel = null!,
			CallbackDelegate cancelCallback = null!,
			object xamlContent = null!
		)
		{
			var popup = new SharedPopup();
			var vs = new VerticalStackLayout()
			{
				Spacing = 10,
			};
			vs.Add(new Label { Text = title, FontSize = 24, FontAttributes = FontAttributes.Bold });
			vs.Add(new Label { Text = msg, FontSize = 20 });

			if (okCallback != null)
			{
				OkCallback = okCallback;
			}

			if (cancelCallback != null)
			{
				CancelCallback = cancelCallback;
			}

			context = new
			{
				Content = xamlContent != null ? xamlContent : vs,
				OkText = ok,
				CancelText = cancel,
				OkCommand = OkCommandDef,
				CancelCommand = CancelCommandDef,
				OkVisible = true,
				CancelVisible = cancelCallback != null ? true : false,
			};
			popup.BindingContext = context;
			await PopupNavigation.PushAsync(popup);
			var res = await popup.PopupDismissedTask;
			return res;
		}

		public static ICommand ShowPopup => new Command(async () =>
		{

			await ConstructPopup("Test", "Testing everything...", "Toast now!", () => _ = AppUIHelpers.LongToast("It works!!!"));
		});

		public static ICommand ClosePopup => new Command(() =>
		{
			PopupNavigation.PushAsync(new LoadingPopup());
		});

		public static ICommand ShowLoading => new Command(() =>
		{
			PopupNavigation.PushAsync(new LoadingPopup());
		});

		public static ICommand HideLoading => new Command(() =>
		{
			MopupService.Instance.PopAsync();
		});

		public static ICommand ShowLoading2 => new Command(async () =>
		{
			using (await Loading.Show())
			{
				if (LoadingPopup.Instance != null)
					LoadingPopup.Instance.BindingContext = new
					{
						Text = "Processing..."
					};
				await Task.Delay(4000);
			}
		});

		public static ICommand HideLoading2 => new Command(() =>
		{
			MopupService.Instance.PopAsync();
		});

		public static async Task OpenLoading(string text = "Loading...")
		{
			try
			{
				await CloseLoading();
				var loadingPopup = new LoadingPopup();
				await PopupNavigation!.PushAsync(loadingPopup);
				loadingPopup.BindingContext = new
				{
					Text = text
				};
			}
			catch (Exception e)
			{
				_ = e.Message;
			}
		}

		public static async Task CloseLoading()
		{
            try
            {
				if (MopupService.Instance.PopupStack.Count < 1) return;
				await PopupNavigation!.PopAsync();
            }
            catch (Exception e)
            {
				_ = e.Message;
			}
        }
	}
}
