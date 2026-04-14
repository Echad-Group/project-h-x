using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile.Pages;

public partial class IssuesListPage : ContentPage
{
    private IssuesListViewModel? _viewModel;

    public IssuesListPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        _viewModel = BindingContext as IssuesListViewModel;
        if (_viewModel != null)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _viewModel.LoadAsync();
            });
        }
    }
}
