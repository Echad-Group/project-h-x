namespace ProjectHX.Mobile.Pages;

public partial class TasksPage : ContentPage
{
    private readonly ViewModels.TasksViewModel _viewModel;

    public TasksPage(ViewModels.TasksViewModel viewModel)
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
