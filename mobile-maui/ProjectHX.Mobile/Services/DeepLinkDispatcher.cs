namespace ProjectHX.Mobile.Services;

public static class DeepLinkDispatcher
{
    public static event Func<Uri, Task>? DeepLinkReceived;

    public static Task DispatchAsync(Uri uri)
    {
        return DeepLinkReceived?.Invoke(uri) ?? Task.CompletedTask;
    }
}
