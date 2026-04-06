namespace ProjectHX.Mobile.Contexts;

public partial class AppStorageContext
{
    public partial string CreateUserRootFolder(string folderName)
    {
        var absolutePath = Path.Combine(FileSystem.Current.AppDataDirectory, folderName);
        Directory.CreateDirectory(absolutePath);
        return absolutePath;
    }

    public partial string GetDataFolderPath()
    {
        return FileSystem.Current.AppDataDirectory;
    }

    public partial string GetUserRootFolderPath(string folderName)
    {
        return Path.Combine(FileSystem.Current.AppDataDirectory, folderName);
    }
}
