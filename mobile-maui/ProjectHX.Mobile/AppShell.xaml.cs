using ProjectHX.Mobile.Pages;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile;

public partial class AppShell : Shell
{
    private readonly ISessionService _sessionService;
    private bool _isRedirecting;

    public AppShell(ISessionService sessionService)
    {
        _sessionService = sessionService;

        InitializeComponent();

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));
        Routing.RegisterRoute(nameof(ResetPasswordPage), typeof(ResetPasswordPage));
        Routing.RegisterRoute(nameof(OtpChallengePage), typeof(OtpChallengePage));
        Routing.RegisterRoute(nameof(TasksPage), typeof(TasksPage));
        Routing.RegisterRoute(nameof(SubmitResultPage), typeof(SubmitResultPage));
        Routing.RegisterRoute(nameof(InboxPage), typeof(InboxPage));
        Routing.RegisterRoute(nameof(LeaderboardPage), typeof(LeaderboardPage));
        Routing.RegisterRoute(nameof(ContentHubPage), typeof(ContentHubPage));
        Routing.RegisterRoute(nameof(NewsDetailPage), typeof(NewsDetailPage));
        Routing.RegisterRoute(nameof(EventDetailPage), typeof(EventDetailPage));
        Routing.RegisterRoute(nameof(IssueDetailPage), typeof(IssueDetailPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(VolunteerPage), typeof(VolunteerPage));

        Navigating += OnShellNavigating;
        _sessionService.SessionChanged += OnSessionChanged;
    }

    public async Task InitializeSessionNavigationAsync()
    {
        await NavigateForSessionStateAsync();
    }

    private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        if (_isRedirecting)
        {
            return;
        }

        var target = e.Target?.Location?.OriginalString;
        if (string.IsNullOrWhiteSpace(target))
        {
            return;
        }

        var hasSession = await _sessionService.HasActiveSessionAsync();
        var isMainRoute = target.StartsWith("//main", StringComparison.OrdinalIgnoreCase);
        var isLoginRoute = target.StartsWith("//login", StringComparison.OrdinalIgnoreCase);
        var isAuthAuxRoute = target.Contains(nameof(RegisterPage), StringComparison.OrdinalIgnoreCase) ||
            target.Contains(nameof(ForgotPasswordPage), StringComparison.OrdinalIgnoreCase) ||
            target.Contains(nameof(ResetPasswordPage), StringComparison.OrdinalIgnoreCase) ||
            target.Contains(nameof(OtpChallengePage), StringComparison.OrdinalIgnoreCase);

        if (isMainRoute && !hasSession)
        {
            e.Cancel();
            _ = RedirectAsync("//login");
            return;
        }

        if ((isLoginRoute || isAuthAuxRoute) && hasSession)
        {
            e.Cancel();
            _ = RedirectAsync("//main");
        }
    }

    private async void OnSessionChanged(object? sender, SessionChangedEventArgs e)
    {
        if (e.HasActiveSession)
        {
            return;
        }

        await RedirectAsync("//login");
    }

    private async Task NavigateForSessionStateAsync()
    {
        var hasSession = await _sessionService.HasActiveSessionAsync();
        await RedirectAsync(hasSession ? "//main" : "//welcome");
    }

    private async Task RedirectAsync(string route)
    {
        if (_isRedirecting)
        {
            return;
        }

        try
        {
            _isRedirecting = true;
            await MainThread.InvokeOnMainThreadAsync(async () => await GoToAsync(route));
        }
        finally
        {
            _isRedirecting = false;
        }
    }
}
