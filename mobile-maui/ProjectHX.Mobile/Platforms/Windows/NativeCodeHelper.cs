namespace ProjectHX.Mobile.Helpers;

public partial class NativeCodeHelper
{
    public static partial void RestartApplication()
    {
        App.Current?.Quit();
    }

    public static partial void StartDownloadService(string url, string destinationPath, string downloadName)
    {
    }

    public static partial void StopDownloadService(string url, string destinationPath, string downloadName)
    {
    }

    public static partial void SetStyleColorLightContent(string color)
    {
    }

    public static partial void SetStyleColorDarkContent(string color)
    {
    }

    public static partial void SetStyleColorLightContent(string statusBarColor, string systemNavBarColor)
    {
    }

    public static partial void SetStyleColorDarkContent(string statusBarColor, string systemNavBarColor)
    {
    }

    public static partial void StatusBarColorWithLightContent(string color)
    {
    }

    public static partial void StatusBarColorWithDarkContent(string color)
    {
    }

    public static partial void SetSystemNavBarColor(string color)
    {
    }
}
