using ProjectHX.Mobile.Services;
using ProjectHX.Mobile.Infrastructure.Outbox;

namespace ProjectHX.Mobile;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISyncOutboxService _outboxService;

    public App(IServiceProvider serviceProvider, ISyncOutboxService outboxService)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _outboxService = outboxService;
        DeepLinkDispatcher.DeepLinkReceived += HandleDeepLinkAsync;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = _serviceProvider.GetRequiredService<AppShell>();
        var window = new Window(shell);
        _ = MainThread.InvokeOnMainThreadAsync(shell.InitializeSessionNavigationAsync);
        _ = Task.Run(async () =>
        {
            await _outboxService.InitializeAsync();
            await _outboxService.ProcessAsync();
        });
        return window;
    }

    protected override void OnResume()
    {
        base.OnResume();
        _ = Task.Run(() => _outboxService.ProcessAsync());
    }

    protected override async void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);
        await HandleDeepLinkAsync(uri);
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
            return;
        }

        await MainThread.InvokeOnMainThreadAsync(async () => await Shell.Current.GoToAsync(route));
    }

    private static string? ResolveDeepLinkRoute(Uri uri)
    {
        var segments = uri.AbsolutePath.Trim('/');
        var target = !string.IsNullOrWhiteSpace(segments) ? segments : uri.Host;
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

        return null;
    }
}
