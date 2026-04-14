using Newtonsoft.Json;
using System.Collections.ObjectModel;
using ProjectHX.Mobile;
using ProjectHX.Models.Configuration;
using ProjectHX.Models.Data;
using AppInfo = ProjectHX.Models.Configuration.AppInfo;

namespace ProjectHX.Mobile.Contexts
{
	public partial class AppStorageContext
	{
		public AppStorageContext(AppInfo appInfo, ServerAppDataFolders serverAppDataFolders)
		{
			AppName = appInfo.Name;
			DatabaseFilename = appInfo.DatabaseFilename;

			TokenKey = appInfo.TokenKey;

			UserRootFolder = CreateUserRootFolder(appInfo.Name);
			AppDataFolder = GetDataFolderPath();

			DatabasePath = Path.Combine(UserRootFolder, "database");
			DocumentPath = Path.Combine(UserRootFolder, "documents");
			FeedbackPath = Path.Combine(UserRootFolder, "feedback");
			FeedbacksFile = Path.Combine(FeedbackPath, "feedbacks.json");
			ImagesPath = Path.Combine(UserRootFolder, "saved_images");
			ProfileImagePath = Path.Combine(ImagesPath, "profile_image");
			SubjectCoverImagesPath = Path.Combine(ImagesPath, "subject_cover_images");
			PastPapersPath = Path.Combine(DocumentPath, "past_papers");
			PastPaperSolutionsPath = Path.Combine(DocumentPath, "past_paper_solutions");
			SubjectNotesPath = Path.Combine(DocumentPath, "subject_notes");

			SqliteDatabasePath = Path.Combine(DatabasePath, DatabaseFilename);
			EnsureDataFoldersExist();
		}
		public string AppName { get; private set; }
		public partial string CreateUserRootFolder(string folderName);
		public partial string GetDataFolderPath();
		public partial string GetUserRootFolderPath(string folderName);
		public void WriteFileToStorage(string fileName, string content, string savePath)
		{
			File.WriteAllText(Path.Combine(savePath, fileName), content);
		}
		private void EnsureDataFoldersExist()
		{
			try
			{
				Directory.CreateDirectory(UserRootFolder);
				Directory.CreateDirectory(DatabasePath);
				Directory.CreateDirectory(DocumentPath);
				Directory.CreateDirectory(ImagesPath);
				Directory.CreateDirectory(PastPapersPath);
				Directory.CreateDirectory(PastPaperSolutionsPath);
				Directory.CreateDirectory(SubjectNotesPath);
				Directory.CreateDirectory(ProfileImagePath);
				Directory.CreateDirectory(SubjectCoverImagesPath);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Failed to initialize application storage: {ex.Message}", ex);
			}
		}
		public string ApplicationDataFolder { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		public string AppDataFolder { get; private set; } = FileSystem.AppDataDirectory;
		public string AppCacheFolder { get; private set; } = FileSystem.CacheDirectory;
		public string UserRootFolder { get; private set; }
		public string DatabasePath { get; private set; }
		public string DocumentPath { get; private set; }
		public string FeedbackPath { get; private set; }
		public string FeedbacksFile { get; private set; }
		public string PastPapersPath { get; private set; }
		public string PastPaperSolutionsPath { get; private set; }
		public string SubjectNotesPath { get; private set; }
		public string ImagesPath { get; private set; }
		public string ProfileImagePath { get; private set; }
		public string SubjectCoverImagesPath { get; private set; }

		public static string PageKey { get; set; } = "PageKey";
		public static string RsGuIdKey { get; set; } = "RsGuIdKey";
		public string UserIdKey { get; set; } = "UserId";
		public string CountryIdKey { get; set; } = "CountryIdKey";
		public string TokenKey { get; set; } = "UserToken";
		public string RoleKey { get; set; } = "RoleKey";
		public string CurrentCityKey { get; set; } = "CurrentCityKey";
		public string PersonalDetailsKey { get; set; } = "PersonalDetailsKey";
		public string IdCardKey { get; set; } = "IdCardKey";
		public string DriverInfoKey { get; set; } = "DriverInfoKey";
		public string TaxiKey { get; set; } = "TaxiKey";

        public string UserIsBoarded { get; set; } = "UserIsBoarded";
		public string UserClassKey { get; set; } = "UserClassKey";
		public string UserDPKey { get; set; } = "UserDPKey";
		public string UserDataLoadedKey { get; set; } = "UserDataLoadedKey";
		public string ShouldNotifyRevisionKey { get; set; } = "ShouldNotifyRevisionKey";

		public bool IsLoginVisible { get; set; } = false;
		public string IsLoginVisibleKey { get; set; } = "IsLoginVisible";
		public bool IsVisible { get; set; } = false;
		public string IsVisibleKey { get; set; } = "IsVisible";

		public string DatabaseFilename { get; private set;}
		/*public const SQLite.SQLiteOpenFlags Flags =
			SQLite.SQLiteOpenFlags.ReadWrite |
			SQLite.SQLiteOpenFlags.Create |
			SQLite.SQLiteOpenFlags.SharedCache;*/
		public static string SqliteDatabasePath { get; private set; } = string.Empty;
	}
}
