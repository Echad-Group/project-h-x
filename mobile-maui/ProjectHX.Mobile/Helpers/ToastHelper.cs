using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace ProjectHX.Mobile.Helpers
{
	public class ToastHelper
	{
		public static async void ShortToast(string message)
		{
			CancellationTokenSource cancellationTokenSource = new();
			ToastDuration duration = ToastDuration.Short;
			double fontSize = 14;
			var toast = Toast.Make(message, duration, fontSize);
			await toast.Show(cancellationTokenSource.Token);
		}
		public static async void LongToast(string message)
		{
			CancellationTokenSource cancellationTokenSource = new();
			ToastDuration duration = ToastDuration.Long;
			double fontSize = 14;
			var toast = Toast.Make(message, duration, fontSize);
			await toast.Show(cancellationTokenSource.Token);
		}
	}
}
