namespace ProjectHX.Mobile.Helpers;

public partial class FileHelper
{
    public static partial string GetFileUri(string filePath)
    {
        return new Uri(filePath, UriKind.Absolute).AbsoluteUri;
    }
}
