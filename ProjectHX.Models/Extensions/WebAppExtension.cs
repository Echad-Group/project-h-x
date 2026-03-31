

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectHX.Contexts;
using ProjectHX.Models.Configuration;

namespace ProjectHX.Extensions
{
    public static class WebAppExtension
	{
		public static string GetConnectionString(this IConfiguration configuration)
		{
			string currentConnectionStringName = configuration.GetRequiredSection("CurrentConnectionString").Value!;
			return configuration.GetConnectionString(currentConnectionStringName)!;
        }

        public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            MongoDbSettings mongoDbSettings = configuration.GetRequiredSection(nameof(MongoDbSettings)).Get<MongoDbSettings>()!;
            services.AddScoped<MongoDbSettings>(sp => mongoDbSettings);

        }

        public static void AddModels(this IServiceCollection services, IConfiguration configuration)
		{

            AddMongoDb(services, configuration);

            // Register Utility classes
            services.AddScoped<HostUrls>(sp => {
                string currentHost = configuration.GetRequiredSection("CurrentHostUrl").Value!;
                var hostUrl = configuration.GetRequiredSection(nameof(HostUrls)).Get<HostUrls>()!;
                hostUrl.Current = configuration.GetSection(nameof(HostUrls))[currentHost]!;
                hostUrl.CurrentApi = $"{hostUrl.Current}/api";
                return hostUrl;
            });

            services.AddScoped<ServerAppDataFolders>(sp => new(configuration.GetRequiredSection(nameof(ServerAppDataFolders)).Get<ServerAppDataFolders>()!));
            services.AddScoped<AppInfo>(sp =>
            {
                return configuration.GetRequiredSection(nameof(AppInfo)).Get<AppInfo>()!;
            });
            services.AddScoped<JwtConfig>(sp =>
            {
                return configuration.GetRequiredSection(nameof(JwtConfig)).Get<JwtConfig>()!;
            });
            services.AddScoped<GoogleConfig>(sp =>
            {
                return configuration.GetRequiredSection(nameof(GoogleConfig)).Get<GoogleConfig>()!;
            });
            services.AddScoped<MailingConfig>(sp =>
            {
                return configuration.GetRequiredSection(nameof(MailingConfig)).Get<MailingConfig>()!;
            });
            services.AddScoped<MailerSendTemplateIds>(sp =>
            {
                return configuration.GetRequiredSection(nameof(MailerSendTemplateIds)).Get<MailerSendTemplateIds>()!;
            });
            services.AddScoped<AppStorageContext>();


        }
	}
}
