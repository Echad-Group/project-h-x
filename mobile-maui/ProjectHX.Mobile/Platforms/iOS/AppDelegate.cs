using Foundation;
using ProjectHX.Mobile.Services;
using UIKit;

namespace ProjectHX.Mobile;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
    {
        if (Uri.TryCreate(url.AbsoluteString, UriKind.Absolute, out var uri))
        {
            _ = DeepLinkDispatcher.DispatchAsync(uri);
        }

        return true;
    }
}

