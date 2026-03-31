using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHX.Models.Configuration
{
    public class AppInfo
    {
        public AppInfo()
        {

        }
        public AppInfo(AppInfo appInfo)
        {
            Name = appInfo.Name;
            Organization = appInfo.Organization;
            Description = appInfo.Description;
            VersionNumber = appInfo.VersionNumber;
            VersionName = appInfo.VersionName;
            AndroidPackageName = appInfo.AndroidPackageName;
            DatabaseFilename = appInfo.DatabaseFilename;
            SkylonEmails = appInfo.SkylonEmails;
            TokenKey = appInfo.TokenKey;
            UserIdKey = $"User_{appInfo.TokenKey}";
            EncryptionKey = appInfo.EncryptionKey;
        }
        public string Name { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VersionNumber { get; set; } = string.Empty;
        public string VersionName { get; set; } = string.Empty;
        public string AndroidPackageName { get; set; } = string.Empty;
        public string DatabaseFilename { get; set; } = string.Empty;
        public Dictionary<string, string> SkylonEmails { get; set; } = new();
        public string TokenKey { get; set; } = string.Empty;
        public string UserIdKey { get; set; } = string.Empty;
        public string EncryptionKey { get; set; } = string.Empty;
    }
}
