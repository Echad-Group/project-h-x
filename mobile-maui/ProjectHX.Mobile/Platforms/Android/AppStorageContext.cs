namespace ProjectHX.Mobile.Contexts
{
	public partial class AppStorageContext
	{
		public partial string CreateUserRootFolder(string folderName)
		{
			//var docsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var docsDirectory = Android.App.Application.Context.GetExternalFilesDir(null)!.ToString();//*Android.App.Application.Context.GetExternalFilesDir(*/Android.OS.Environment.DirectoryDocuments/*)*/;
			string absolutePath = $"{docsDirectory/*.AbsoluteFile.Path*/}/{folderName}";
			if(!Directory.Exists(absolutePath))
			{
				Directory.CreateDirectory(absolutePath);
			}
			return absolutePath;
		}
		public partial string GetDataFolderPath()
		{
			return Android.App.Application.Context.GetExternalFilesDir(null)!.ToString();
		}
		public partial string GetUserRootFolderPath(string folderName)
		{
			var docsDirectory = Android.App.Application.Context.GetExternalFilesDir(null)!.ToString();//*Android.App.Application.Context.GetExternalFilesDir(*/Android.OS.Environment.DirectoryDocuments/*)*/;
			string absolutePath = $"{docsDirectory/*.AbsoluteFile.Path*/}/{folderName}";
			return absolutePath;
		}
	}
}
