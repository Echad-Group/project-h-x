namespace ProjectHX.Mobile.ViewModels;

public interface IAsyncPageLoadable
{
    Task LoadAsync(CancellationToken cancellationToken = default);
}
