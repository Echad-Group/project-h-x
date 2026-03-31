using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mopups.Interfaces;
using Mopups.Services;
using ProjectHX.Extensions;
using ProjectHX.Mobile.Contexts;
using ProjectHX.Mobile.Infrastructure.Diagnostics;
using ProjectHX.Mobile.Infrastructure.Outbox;
using ProjectHX.Mobile.Pages;
using ProjectHX.Mobile.Services;
using ProjectHX.Mobile.Services.Interfaces;
using ProjectHX.Mobile.ViewModels;
using ProjectHX.Models.Configuration;
using AppInfo = ProjectHX.Models.Configuration.AppInfo;

namespace ProjectHX.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        using (var appSettingsStream = FileSystem.OpenAppPackageFileAsync("appsettings.json").GetAwaiter().GetResult())
        {
            builder.Configuration.AddJsonStream(appSettingsStream);
            builder.Services.AddModels(builder.Configuration);
        }

        builder.Logging.SetMinimumLevel(LogLevel.Information);
        builder.Logging.AddDebug();

        builder.Services.AddSingleton<IApiBaseUrlProvider, ApiBaseUrlProvider>();
        builder.Services.AddSingleton<IAppNavigator, AppNavigator>();
        builder.Services.AddSingleton<ISessionService, SessionService>();
        builder.Services.AddSingleton<IAuthFlowStateService, AuthFlowStateService>();
        builder.Services.AddSingleton<IAppDiagnosticsService, AppDiagnosticsService>();
        builder.Services.AddTransient<AuthTokenHandler>();

        // Set up HttpClient with base address from config
        builder.Services
            .AddHttpClient("ProjectHxApi", (serviceProvider, client) =>
            {
                client.BaseAddress = serviceProvider.GetRequiredService<IApiBaseUrlProvider>().GetBaseUri();
                if (client.BaseAddress.Host.Contains("ngrok", StringComparison.OrdinalIgnoreCase))
                    client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
            })
            .AddHttpMessageHandler<AuthTokenHandler>();

        /*builder.Services.AddSingleton<HttpClient>(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("ProjectHxApi");
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var apiUrls = config.GetRequiredSection(nameof(ApiUrls)).Get<ApiUrls>()!;

            client.BaseAddress = new Uri(apiUrls.NgRokTmpHost);
            if(client.BaseAddress.Host.Contains("ngrok", StringComparison.OrdinalIgnoreCase))
                client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");

            return client;
        });*/


        builder.Services.AddSingleton<IPopupNavigation>(MopupService.Instance);


        builder.Services.AddScoped<AppHttpContext>(serviceProvider =>
        {
            var baseUri = serviceProvider.GetRequiredService<IApiBaseUrlProvider>().GetBaseUri();
            return new(baseUri.GetLeftPart(UriPartial.Authority), baseUri.ToString(), new HttpClient());
        });
        builder.Services.AddScoped<ServerAppDataFolders>(sp => new(builder.Configuration.GetRequiredSection(nameof(ServerAppDataFolders)).Get<ServerAppDataFolders>()!));
        builder.Services.AddTransient<AppStorageContext>(serviceProvider =>
        {
            var appInfo = builder.Configuration.GetRequiredSection(nameof(AppInfo)).Get<AppInfo>()!;
            var dataFolders = builder.Configuration.GetRequiredSection(nameof(ServerAppDataFolders)).Get<ServerAppDataFolders>()!;
            return new(appInfo, dataFolders);
        });
        builder.Services.AddScoped<AppInfo>(serviceProvider =>
        {
            var appInfo = builder.Configuration.GetRequiredSection(nameof(AppInfo)).Get<AppInfo>()!;
            return new(appInfo);
        });

        builder.Services.AddScoped<SqliteDbContext>();

        builder.Services.AddSingleton<IAuthApiService, AuthApiService>();
        builder.Services.AddSingleton<IUserProfileApiService, UserProfileApiService>();
        builder.Services.AddSingleton<IVolunteerApiService, VolunteerApiService>();
        builder.Services.AddSingleton<ISyncOutboxService, SyncOutboxService>();
        builder.Services.AddSingleton<ITasksApiService, TasksApiService>();
        builder.Services.AddSingleton<IInboxApiService, InboxApiService>();
        builder.Services.AddSingleton<ILeaderboardApiService, LeaderboardApiService>();
        builder.Services.AddSingleton<IResultsApiService, ResultsApiService>();
        builder.Services.AddSingleton<INewsApiService, NewsApiService>();
        builder.Services.AddSingleton<IEventsApiService, EventsApiService>();
        builder.Services.AddSingleton<IIssuesApiService, IssuesApiService>();
        builder.Services.AddSingleton<ICampaignTeamApiService, CampaignTeamApiService>();
        builder.Services.AddSingleton<IPushApiService, PushApiService>();

        builder.Services.AddTransient<AppShell>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<ForgotPasswordViewModel>();
        builder.Services.AddTransient<OtpChallengeViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<ResetPasswordViewModel>();
        builder.Services.AddTransient<TasksViewModel>();
        builder.Services.AddTransient<InboxViewModel>();
        builder.Services.AddTransient<LeaderboardViewModel>();
        builder.Services.AddTransient<SubmitResultViewModel>();
        builder.Services.AddTransient<ContentHubViewModel>();
        builder.Services.AddTransient<NewsDetailViewModel>();
        builder.Services.AddTransient<EventDetailViewModel>();
        builder.Services.AddTransient<IssueDetailViewModel>();

        builder.Services.AddTransient<Pages.LoginPage>();
        builder.Services.AddTransient<Pages.RegisterPage>();
        builder.Services.AddTransient<Pages.ForgotPasswordPage>();
        builder.Services.AddTransient<Pages.OtpChallengePage>();
        builder.Services.AddTransient<Pages.ResetPasswordPage>();
        builder.Services.AddTransient<Pages.TasksPage>();
        builder.Services.AddTransient<Pages.SubmitResultPage>();
        builder.Services.AddTransient<Pages.InboxPage>();
        builder.Services.AddTransient<Pages.LeaderboardPage>();
        builder.Services.AddTransient<Pages.ContentHubPage>();
        builder.Services.AddTransient<Pages.NewsDetailPage>();
        builder.Services.AddTransient<Pages.EventDetailPage>();
        builder.Services.AddTransient<Pages.IssueDetailPage>();
        builder.Services.AddTransient<Pages.ProfilePage>();
        builder.Services.AddTransient<LoadingPage>();
        builder.Services.AddTransient<OnboardingPage>();

        var app = builder.Build();

        // Eagerly initialize the local sqlite store once on startup.
        using (var scope = app.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<SqliteDbContext>().Database.EnsureCreated();
        }

        return app;
    }
}
