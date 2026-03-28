using ProjectHX.Mobile.Pages;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile;

public partial class AppShell : Shell
{
    private readonly ISessionService _sessionService;

    public AppShell(ISessionService sessionService)
    {
        _sessionService = sessionService;

        InitializeComponent();

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(OtpChallengePage), typeof(OtpChallengePage));
        Routing.RegisterRoute(nameof(TasksPage), typeof(TasksPage));
        Routing.RegisterRoute(nameof(SubmitResultPage), typeof(SubmitResultPage));
        Routing.RegisterRoute(nameof(InboxPage), typeof(InboxPage));
        Routing.RegisterRoute(nameof(LeaderboardPage), typeof(LeaderboardPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));

        Navigating += OnShellNavigating;
    }

    public async Task InitializeSessionNavigationAsync()
    {
        var hasSession = await _sessionService.HasActiveSessionAsync();
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await GoToAsync(hasSession ? "//main" : "//login");
        });
    }

    private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        var target = e.Target?.Location?.OriginalString;
        if (string.IsNullOrWhiteSpace(target))
        {
            return;
        }

        var hasSession = await _sessionService.HasActiveSessionAsync();
        var isMainRoute = target.StartsWith("//main", StringComparison.OrdinalIgnoreCase);
        var isLoginRoute = target.StartsWith("//login", StringComparison.OrdinalIgnoreCase);

        if (isMainRoute && !hasSession)
        {
            e.Cancel();
            _ = MainThread.InvokeOnMainThreadAsync(async () => await GoToAsync("//login"));
            return;
        }

        if (isLoginRoute && hasSession)
        {
            e.Cancel();
            _ = MainThread.InvokeOnMainThreadAsync(async () => await GoToAsync("//main"));
        }
    }
}
