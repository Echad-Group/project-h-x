namespace ProjectHX.Mobile.Pages;

public partial class SubmitResultPage : ContentPage
{
    private readonly ViewModels.SubmitResultViewModel _viewModel;

    public SubmitResultPage(ViewModels.SubmitResultViewModel viewModel)
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
