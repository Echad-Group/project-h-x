namespace ProjectHX.Mobile.Pages;

public partial class LeaderboardPage : ContentPage
{
    private readonly ViewModels.LeaderboardViewModel _viewModel;
    private CancellationTokenSource? _loadCancellationTokenSource;

    public LeaderboardPage(ViewModels.LeaderboardViewModel viewModel)
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
