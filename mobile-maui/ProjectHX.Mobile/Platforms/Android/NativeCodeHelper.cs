using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;

namespace ProjectHX.Mobile.Helpers
{
	public partial class NativeCodeHelper
	{
		public static partial void RestartApplication()
		{
			try
			{
				var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
				var pm = activity!.PackageManager;
				var intent = pm!.GetLaunchIntentForPackage(activity.PackageName!);
				intent!.AddFlags(Android.Content.ActivityFlags.ClearTop | Android.Content.ActivityFlags.NewTask);
				activity.FinishAffinity();
				activity.StartActivity(intent);
				// Java.Lang.JavaSystem.Exit(0);
			}
			catch (Exception)
			{
				// Optionally log or handle the error
				App.Current!.Quit();
			}
		}
		public static partial void StartDownloadService(string url, string destinationPath, string downloadName)
		{

		}
		public static partial void StopDownloadService(string url, string destinationPath, string downloadName)
		{

		}
		public static partial void SetStyleColorLightContent(string color)
		{
			Shell.Current.Behaviors.Add(new StatusBarBehavior
			{
				StatusBarColor = Color.FromArgb(color),
				StatusBarStyle = StatusBarStyle.LightContent,
			});
			SetSystemNavBarColor(color);
		}
		public static partial void SetStyleColorDarkContent(string color)
		{
			Shell.Current.Behaviors.Add(new StatusBarBehavior
			{
				StatusBarColor = Color.FromArgb(color),
				StatusBarStyle = StatusBarStyle.DarkContent,
			});
			SetSystemNavBarColor(color);
		}
		public static partial void SetStyleColorLightContent(string statusBarColor, string systemNavBarColor)
		{
			Shell.Current.Behaviors.Add(new StatusBarBehavior
			{
				StatusBarColor = Color.FromArgb(statusBarColor),
				StatusBarStyle = StatusBarStyle.LightContent,
			});
			SetSystemNavBarColor(systemNavBarColor);
		}
		public static partial void SetStyleColorDarkContent(string statusBarColor, string systemNavBarColor)
		{
			Shell.Current.Behaviors.Add(new StatusBarBehavior
			{
				StatusBarColor = Color.FromArgb(statusBarColor),
				StatusBarStyle = StatusBarStyle.DarkContent,
			});
			SetSystemNavBarColor(systemNavBarColor);
		}

		public static partial void StatusBarColorWithLightContent(string color)
		{
			Shell.Current.Behaviors.Add(new StatusBarBehavior
			{
				StatusBarColor = Color.FromArgb(color),
				StatusBarStyle = StatusBarStyle.LightContent,
			});
		}

		public static partial void StatusBarColorWithDarkContent(string color)
		{
			Shell.Current.Behaviors.Add(new StatusBarBehavior
			{
				StatusBarColor = Color.FromArgb(color),
				StatusBarStyle = StatusBarStyle.LightContent,
			});
		}

		public static partial void SetSystemNavBarColor(string color)
		{
			var mainActivityInstance = MainActivity.Instance;
			var androidStaticHelperMethods = mainActivityInstance!.AndroidStaticHelperMethods;
			androidStaticHelperMethods.SetNavigationBarColor(color);
		}

		/*public static partial void StartLocationBackgroundService()
		{
#if ANDROID
			var context = Android.App.Application.Context;
			var intent = new Android.Content.Intent(context, typeof(TaxiFinder.Mobile.Platforms.Android.LocationAndroidBackgroundService));
#pragma warning disable CA1416 // Validate platform compatibility
			context.StartForegroundService(intent);
#pragma warning restore CA1416 // Validate platform compatibility
#endif
		}
		public static partial void StopLocationBackgroundService()
		{
#if ANDROID
			var context = Android.App.Application.Context;
			var intent = new Android.Content.Intent(context, typeof(TaxiFinder.Mobile.Platforms.Android.LocationAndroidBackgroundService));
			context.StopService(intent);
#endif
		}*/
	}
}
