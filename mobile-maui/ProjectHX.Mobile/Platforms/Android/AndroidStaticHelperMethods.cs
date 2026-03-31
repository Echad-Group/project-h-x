using Android.App;
using Microsoft.Maui.Platform;

namespace ProjectHX.Mobile
{
	public class AndroidStaticHelperMethods
	{
		public static AndroidStaticHelperMethods? Instance { get; private set; }
		public AndroidStaticHelperMethods(Activity activity)
		{
			CurrentActivity = activity;
			Instance = this;
		}
		public static Activity? CurrentActivity;
		public void SetNavigationBarColor(string color)
		{
			Color mauiColor = Color.FromArgb(color);
#pragma warning disable CA1422 // Validate platform compatibility
			CurrentActivity!.Window!.SetNavigationBarColor(mauiColor.ToPlatform());
#pragma warning restore CA1422 // Validate platform compatibility
		}
	}
}
