using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile.Pages;

public partial class IssueDetailPage : ContentPage, IQueryAttributable
{
    private readonly IssueDetailViewModel _viewModel;

    public IssueDetailPage(IssueDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var idObj) && int.TryParse(idObj?.ToString(), out var id))
        {
            await _viewModel.LoadByIdAsync(id);
            return;
        }

        if (query.TryGetValue("slug", out var slugObj) && slugObj is string slug)
        {
            await _viewModel.LoadBySlugAsync(Uri.UnescapeDataString(slug));
        }
    }
}
