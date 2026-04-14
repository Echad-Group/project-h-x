using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile.Pages;

public partial class ContentHubPage : ContentPage
{
    private readonly ContentHubViewModel _viewModel;
    private CancellationTokenSource? _loadCancellationTokenSource;

    public ContentHubPage(ContentHubViewModel viewModel)
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
