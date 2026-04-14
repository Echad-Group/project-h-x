using ProjectHX.Mobile.ViewModels;

namespace ProjectHX.Mobile.Pages;

internal static class PageLoadCoordinator
{
    public static void StartLoad(IAsyncPageLoadable loadableViewModel, BaseViewModel? baseViewModel, ref CancellationTokenSource? cancellationTokenSource)
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
        cancellationTokenSource = new CancellationTokenSource();

        _ = LoadSafelyAsync(loadableViewModel, baseViewModel, cancellationTokenSource.Token);
    }

    public static void CancelLoad(ref CancellationTokenSource? cancellationTokenSource)
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
        cancellationTokenSource = null;
    }

    private static async Task LoadSafelyAsync(IAsyncPageLoadable loadableViewModel, BaseViewModel? baseViewModel, CancellationToken cancellationToken)
    {
        try
        {
            await loadableViewModel.LoadAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex) when (baseViewModel is not null)
        {
            baseViewModel.ErrorMessage = ex.Message;
        }
    }
}
