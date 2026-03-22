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

        builder.Services.AddSingleton<HttpClient>(_ => new HttpClient
        {
            BaseAddress = new Uri("https://6a27-102-244-197-164.ngrok-free.app/api/")
        });

        builder.Services.AddSingleton<ISessionService, SessionService>();
        builder.Services.AddSingleton<IAuthApiService, AuthApiService>();
        builder.Services.AddSingleton<ISyncOutboxService, SyncOutboxService>();

        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<OtpChallengeViewModel>();

        builder.Services.AddTransient<Pages.LoginPage>();
        builder.Services.AddTransient<Pages.OtpChallengePage>();
        builder.Services.AddTransient<Pages.TasksPage>();
        builder.Services.AddTransient<Pages.SubmitResultPage>();
        builder.Services.AddTransient<Pages.InboxPage>();
        builder.Services.AddTransient<Pages.LeaderboardPage>();
        builder.Services.AddTransient<Pages.ProfilePage>();

        return builder.Build();
    }
}
