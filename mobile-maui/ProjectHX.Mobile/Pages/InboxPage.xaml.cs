namespace ProjectHX.Mobile.Pages;

public partial class InboxPage : ContentPage
{
    private readonly ViewModels.InboxViewModel _viewModel;
    private CancellationTokenSource? _loadCancellationTokenSource;

    public InboxPage(ViewModels.InboxViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        PageLoadCoordinator.StartLoad(_viewModel, _viewModel, ref _loadCancellationTokenSource);
    }

    protected override void OnDisappearing()
    {
        PageLoadCoordinator.CancelLoad(ref _loadCancellationTokenSource);
        base.OnDisappearing();
    }
}
