namespace ProjectHX.Models.Configuration
{
    public class ServerAppDataFolders
    {
        public ServerAppDataFolders()
        {
        }
        public ServerAppDataFolders(ServerAppDataFolders dataFolders)
        {
            RootDataFolder = dataFolders.RootDataFolder;
            ProfileImagesFolder = dataFolders.ProfileImagesFolder;
            IdCardsFolder = dataFolders.IdCardsFolder;
            LicensesFolder = dataFolders.LicensesFolder;
            ThumbnailsFolder = dataFolders.ThumbnailsFolder;
            ListingsCoverImagesFolder = dataFolders.ListingsCoverImagesFolder;
            ListingsDataFolder = dataFolders.ListingsDataFolder;
            FeedbackFolder = dataFolders.FeedbackFolder;
            FeedbackPhotosFolder = dataFolders.FeedbackPhotosFolder;
            Temp = dataFolders.Temp;
            Logs = dataFolders.Logs;
            VisitorInfos = dataFolders.VisitorInfos;
        }
        public string RootDataFolder { get; set; } = string.Empty;
        public string ProfileImagesFolder { get; set; } = string.Empty;
        public string IdCardsFolder { get; set; } = string.Empty;
        public string LicensesFolder { get; set; } = string.Empty;
        public string ThumbnailsFolder { get; set; } = string.Empty;
        public string ListingsCoverImagesFolder { get; set; } = string.Empty;
        public string ListingsDataFolder { get; set; } = string.Empty;
        public string FeedbackFolder { get; set; } = string.Empty;
        public string FeedbackPhotosFolder { get; set; } = string.Empty;
        public string Temp { get; set; } = string.Empty;
        public string Logs { get; set; } = string.Empty;
        public string VisitorInfos { get; set; } = string.Empty;
    }
}
