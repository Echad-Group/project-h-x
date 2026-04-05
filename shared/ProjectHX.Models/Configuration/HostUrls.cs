namespace ProjectHX.Models.Configuration
{
    public class HostUrls
    {
        public HostUrls()
        {

        }
        public HostUrls(HostUrls hostUrls)
        {
            CurrentApi = hostUrls.CurrentApi;
            Current = hostUrls.Current;
            Local = hostUrls.Local;
            LocalSecured = hostUrls.LocalSecured;
            LocalSecuredApi = hostUrls.LocalSecuredApi;
            HostApi = hostUrls.HostApi;
            Host = hostUrls.Host;
            NgRokTmpHostApi = hostUrls.NgRokTmpHostApi;
            NgRokTmpHost = hostUrls.NgRokTmpHost;
        }
        public string Current { get; set; } = string.Empty;
        public string CurrentApi { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
        public string LocalApi { get; set; } = string.Empty;
        public string LocalSecured { get; set; } = string.Empty;
        public string LocalSecuredApi { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string HostApi { get; set; } = string.Empty;
        public string NgRokTmpHost { get; set; } = string.Empty;
        public string NgRokTmpHostApi { get; set; } = string.Empty;
    }
}
