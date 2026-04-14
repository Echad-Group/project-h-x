using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile.Pages;

public partial class EventDetailPage : ContentPage, IQueryAttributable
{
    private readonly EventDetailViewModel _viewModel;

    public EventDetailPage(EventDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        try
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
        catch (Exception ex)
        {
            _viewModel.ErrorMessage = ex.Message;
        }
    }
}
