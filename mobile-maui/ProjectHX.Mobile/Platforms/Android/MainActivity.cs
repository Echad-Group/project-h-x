using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using ProjectHX.Mobile.Services;

namespace ProjectHX.Mobile;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(
	new[] { Intent.ActionView },
	Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
	DataScheme = "projecthx",
	AutoVerify = false)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		_ = DispatchIncomingLinkAsync(Intent);
	}

	protected override void OnNewIntent(Intent? intent)
	{
		base.OnNewIntent(intent);
		_ = DispatchIncomingLinkAsync(intent);
	}

	private static Task DispatchIncomingLinkAsync(Intent? intent)
	{
		var uriString = intent?.DataString;
		if (string.IsNullOrWhiteSpace(uriString) || !Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
		{
			return Task.CompletedTask;
		}

		return DeepLinkDispatcher.DispatchAsync(uri);
	}
}
