using Microsoft.Extensions.Logging;
using ProjectHX.Mobile.Infrastructure.Diagnostics;
using ProjectHX.Mobile.Services;
using ProjectHX.Mobile.Infrastructure.Outbox;

namespace ProjectHX.Mobile;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISyncOutboxService _outboxService;
    private readonly IAppDiagnosticsService _diagnosticsService;
    private readonly ILogger<App> _logger;
    private Task? _startupWork;
    private Task? _resumeWork;

    public App(
        IServiceProvider serviceProvider,
        ISyncOutboxService outboxService,
        IAppDiagnosticsService diagnosticsService,
        ILogger<App> logger)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _outboxService = outboxService;
        _diagnosticsService = diagnosticsService;
        _logger = logger;
        DeepLinkDispatcher.DeepLinkReceived += HandleDeepLinkAsync;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = _serviceProvider.GetRequiredService<AppShell>();
        var window = new Window(shell);
        _startupWork = InitializeAppAsync(shell);
        return window;
    }

    protected override void OnResume()
    {
        base.OnResume();
        _resumeWork = ProcessOutboxOnResumeAsync();
    }

    protected override async void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);
        await HandleDeepLinkAsync(uri);
    }

    private async Task InitializeAppAsync(AppShell shell)
    {
        try
        {
            await shell.InitializeSessionNavigationAsync();
            await _outboxService.InitializeAsync();
            await _outboxService.ProcessAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "App startup outbox initialization failed.");
            await _diagnosticsService.RecordEventAsync(
                category: "app",
                eventName: "startup_outbox_failed",
                severity: "Error",
                message: ex.Message,
                context: new { exception = ex.GetType().Name });
        }
    }

    private async Task ProcessOutboxOnResumeAsync()
    {
        try
        {
            await _outboxService.ProcessAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Outbox processing failed on app resume.");
            await _diagnosticsService.RecordEventAsync(
                category: "app",
                eventName: "resume_outbox_failed",
                severity: "Error",
                message: ex.Message,
                context: new { exception = ex.GetType().Name });
        }
    }

    private async Task HandleDeepLinkAsync(Uri uri)
    {
        if (uri is null)
        {
            return;
        }

        var route = ResolveDeepLinkRoute(uri);
        if (string.IsNullOrWhiteSpace(route))
        {
            await _diagnosticsService.RecordEventAsync(
                category: "deeplink",
                eventName: "unmapped_link",
                severity: "Warning",
                message: "Received unsupported deep link.",
                context: new { uri = uri.ToString() });
            return;
        }

        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () => await Shell.Current.GoToAsync(route));
            await _diagnosticsService.RecordEventAsync(
                category: "deeplink",
                eventName: "route_navigation",
                severity: "Info",
                message: "Deep link route navigation succeeded.",
                context: new { route, uri = uri.ToString() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Deep link navigation failed for route {Route}.", route);
            await _diagnosticsService.RecordEventAsync(
                category: "deeplink",
                eventName: "route_navigation_failed",
                severity: "Error",
                message: ex.Message,
                context: new { route, uri = uri.ToString(), exception = ex.GetType().Name });
        }
    }

    private static string? ResolveDeepLinkRoute(Uri uri)
    {
        var segments = uri.AbsolutePath.Trim('/');
        var target = !string.IsNullOrWhiteSpace(segments) ? segments : uri.Host;
        var parts = target.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

        if (target.Equals("reset-password", StringComparison.OrdinalIgnoreCase))
        {
            var email = Uri.EscapeDataString(query["email"] ?? string.Empty);
            var token = Uri.EscapeDataString(query["token"] ?? string.Empty);
            return $"{nameof(Pages.ResetPasswordPage)}?email={email}&token={token}";
        }

        if (target.Equals("otp", StringComparison.OrdinalIgnoreCase))
        {
            var email = Uri.EscapeDataString(query["email"] ?? string.Empty);
            var purpose = Uri.EscapeDataString(query["purpose"] ?? "Login");
            var code = Uri.EscapeDataString(query["code"] ?? string.Empty);
            return $"{nameof(Pages.OtpChallengePage)}?email={email}&purpose={purpose}&code={code}";
        }

        if (parts.Length >= 2 && parts[0].Equals("news", StringComparison.OrdinalIgnoreCase))
        {
            var slugPart = parts[1].Equals("slug", StringComparison.OrdinalIgnoreCase) && parts.Length >= 3
                ? parts[2]
                : parts[1];
            var slug = Uri.EscapeDataString(slugPart);
            return $"{nameof(Pages.NewsDetailPage)}?slug={slug}";
        }

        if (parts.Length >= 2 && parts[0].Equals("events", StringComparison.OrdinalIgnoreCase))
        {
            var eventPart = parts[1].Equals("slug", StringComparison.OrdinalIgnoreCase) && parts.Length >= 3
                ? parts[2]
                : parts[1];
            var eventRef = Uri.EscapeDataString(eventPart);
            return $"{nameof(Pages.EventDetailPage)}?slug={eventRef}";
        }

        if (parts.Length >= 2 && parts[0].Equals("issues", StringComparison.OrdinalIgnoreCase))
        {
            var issuePart = parts[1].Equals("slug", StringComparison.OrdinalIgnoreCase) && parts.Length >= 3
                ? parts[2]
                : parts[1];
            var issueRef = Uri.EscapeDataString(issuePart);
            return $"{nameof(Pages.IssueDetailPage)}?slug={issueRef}";
        }

        if (target.Equals("explore", StringComparison.OrdinalIgnoreCase) || target.Equals("campaignteam", StringComparison.OrdinalIgnoreCase))
        {
            return "//main/explore-tab/explore";
        }

        return null;
    }
}
