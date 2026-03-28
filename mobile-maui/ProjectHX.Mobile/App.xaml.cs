namespace ProjectHX.Mobile;

public partial class App : Application
{
    private readonly AppShell _shell;

    public App(AppShell shell)
    {
        InitializeComponent();
        _shell = shell;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(_shell);
        _ = MainThread.InvokeOnMainThreadAsync(_shell.InitializeSessionNavigationAsync);
        return window;
    }
}
