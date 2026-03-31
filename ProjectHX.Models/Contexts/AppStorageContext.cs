using ProjectHX.Models.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace ProjectHX.Contexts
{
    public partial class AppStorageContext
    {
        public AppStorageContext(AppInfo appInfo, HostUrls hostUrls, ServerAppDataFolders serverAppDataFolders, IHostingEnvironment environment)
        {
            AppName = appInfo.Name;
            foreach (var property in serverAppDataFolders.GetType().GetProperties())
            {
                if (property.CanRead)
                {
                    CreateDataFolder(environment.WebRootPath + $"{property.GetValue(serverAppDataFolders)}");
                }
            }
        }
        public string AppName { get; private set; }
        public void WriteFileToStorage(string fileName, string content, string savePath)
        {
            File.WriteAllText(Path.Combine(savePath, fileName), content);
        }

        public bool CreateDataFolder(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static string PageKey { get; set; } = "PageKey";
        public static string RsGuIdKey { get; set; } = "RsGuIdKey";
        public string UserIdKey { get; set; } = "UserId";
        public string CountryIdKey { get; set; } = "CountryIdKey";
        public string TokenKey { get; set; } = "UserToken";
        public string UserAgread { get; set; } = "UserAgread";
        public string UserClassKey { get; set; } = "UserClassKey";
        public string UserDPKey { get; set; } = "UserDPKey";
        public string UserDataLoadedKey { get; set; } = "UserDataLoadedKey";
        public string ShouldNotifyRevisionKey { get; set; } = "ShouldNotifyRevisionKey";

        public bool IsLoginVisible { get; set; } = false;
        public string IsLoginVisibleKey { get; set; } = "IsLoginVisible";
        public bool IsVisible { get; set; } = false;
        public string IsVisibleKey { get; set; } = "IsVisible";
    }
}
