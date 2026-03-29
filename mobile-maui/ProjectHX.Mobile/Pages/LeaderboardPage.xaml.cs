namespace ProjectHX.Mobile.Pages;

public partial class LeaderboardPage : ContentPage
{
    private readonly ViewModels.LeaderboardViewModel _viewModel;

    public LeaderboardPage(ViewModels.LeaderboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
