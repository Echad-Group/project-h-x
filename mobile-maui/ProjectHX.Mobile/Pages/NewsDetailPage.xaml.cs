using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile.Pages;

public partial class NewsDetailPage : ContentPage, IQueryAttributable
{
    private readonly NewsDetailViewModel _viewModel;

    public NewsDetailPage(NewsDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("slug", out var slugObj) && slugObj is string slug)
        {
            await _viewModel.LoadAsync(Uri.UnescapeDataString(slug));
        }
    }
}
