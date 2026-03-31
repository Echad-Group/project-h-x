using Android.Content;
using Uri = Android.Net.Uri;
using FileProvider = AndroidX.Core.Content.FileProvider;


namespace ProjectHX.Mobile.Helpers
{
	public partial class FileHelper
	{
		public static partial string GetFileUri(string filePath)
		{
			Java.IO.File file = new Java.IO.File(filePath);
			Context context = Android.App.Application.Context;
			Uri fileUri = FileProvider.GetUriForFile(context, $"{context.PackageName}.fileprovider", file)!;
			return fileUri.ToString()!;
		}
	}

}
