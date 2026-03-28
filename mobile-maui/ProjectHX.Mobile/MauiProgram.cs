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
        builder.Services.AddSingleton<ISyncOutboxService, SyncOutboxService>();

        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<OtpChallengeViewModel>();

        builder.Services.AddTransient<Pages.LoginPage>();
        builder.Services.AddTransient<Pages.RegisterPage>();
        builder.Services.AddTransient<Pages.OtpChallengePage>();
        builder.Services.AddTransient<Pages.TasksPage>();
        builder.Services.AddTransient<Pages.SubmitResultPage>();
        builder.Services.AddTransient<Pages.InboxPage>();
        builder.Services.AddTransient<Pages.LeaderboardPage>();
        builder.Services.AddTransient<Pages.ProfilePage>();

        return builder.Build();
    }
}
