using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile.Pages;

public partial class IssuesListPage : ContentPage
{
    private readonly IssuesListViewModel _viewModel;
    private CancellationTokenSource? _loadCancellationTokenSource;

    public IssuesListPage(IssuesListViewModel viewModel)
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
