using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile.Pages;

public partial class EventsListPage : ContentPage
{
    private EventsListViewModel? _viewModel;

    public EventsListPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        _viewModel = BindingContext as EventsListViewModel;
        if (_viewModel != null)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _viewModel.LoadAsync();
            });
        }
    }
}
