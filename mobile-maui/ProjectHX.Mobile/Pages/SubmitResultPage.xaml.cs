namespace ProjectHX.Mobile.Pages;

public partial class SubmitResultPage : ContentPage
{
    private readonly ViewModels.SubmitResultViewModel _viewModel;
    private CancellationTokenSource? _loadCancellationTokenSource;

    public SubmitResultPage(ViewModels.SubmitResultViewModel viewModel)
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
        _viewModel.StopObserving();
        PageLoadCoordinator.CancelLoad(ref _loadCancellationTokenSource);
        base.OnDisappearing();
    }
}
