namespace ProjectHX.Mobile.Models.PublicContent;

public sealed class PushSubscriptionRequestModel
{
    public string Endpoint { get; set; } = string.Empty;
    public PushKeysModel Keys { get; set; } = new();
}

public sealed class PushKeysModel
{
    public string? P256dh { get; set; }
    public string? Auth { get; set; }
}

public sealed class PushStatusModel
{
    public bool IsSubscribed { get; set; }
    public DateTime? SubscriptionDate { get; set; }
}
