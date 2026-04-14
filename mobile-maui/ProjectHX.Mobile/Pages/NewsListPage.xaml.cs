using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile.Pages;

public partial class NewsListPage : ContentPage
{
    private NewsListViewModel? _viewModel;

    public NewsListPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        _viewModel = BindingContext as NewsListViewModel;
        if (_viewModel != null)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _viewModel.LoadAsync();
            });
        }
    }
}
