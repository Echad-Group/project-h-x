namespace ProjectHX.Mobile.Pages;

public partial class InboxPage : ContentPage
{
    private readonly ViewModels.InboxViewModel _viewModel;

    public InboxPage(ViewModels.InboxViewModel viewModel)
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
