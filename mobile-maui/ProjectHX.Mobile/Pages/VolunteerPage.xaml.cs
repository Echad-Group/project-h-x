using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile.Pages;

public partial class VolunteerPage : ContentPage
{
    private readonly VolunteerPageViewModel _viewModel;

    public VolunteerPage(VolunteerPageViewModel viewModel)
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
