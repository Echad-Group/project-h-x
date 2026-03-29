using Microsoft.Extensions.Configuration;
using ProjectHX.Mobile.Infrastructure.Outbox;
using ProjectHX.Mobile.Services;
using ProjectHX.Mobile.Services.Interfaces;
using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        using (var appSettingsStream = FileSystem.OpenAppPackageFileAsync("appsettings.json").GetAwaiter().GetResult())
        {
            builder.Configuration.AddJsonStream(appSettingsStream);
        }

        builder.Services.AddSingleton<IApiBaseUrlProvider, ApiBaseUrlProvider>();
        builder.Services.AddSingleton<ISessionService, SessionService>();
        builder.Services.AddSingleton<IAuthFlowStateService, AuthFlowStateService>();
        builder.Services.AddTransient<AuthTokenHandler>();

        builder.Services
            .AddHttpClient("ProjectHxApi", (serviceProvider, client) =>
            {
                client.BaseAddress = serviceProvider.GetRequiredService<IApiBaseUrlProvider>().GetBaseUri();
            })
            .AddHttpMessageHandler<AuthTokenHandler>();

        builder.Services.AddSingleton<HttpClient>(serviceProvider =>
            serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("ProjectHxApi"));

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

        return builder.Build();
    }
}
