namespace ProjectHX.Mobile.Models.Inbox;

public sealed class InboxMessage
{
    public int Id { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? Url { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }

    public bool IsUnread => ReadAt == null;
    public string TimeDisplay => CreatedAt.ToLocalTime().ToString("d MMM, HH:mm");
    public string ChannelDisplay => Channel?.ToUpperInvariant() ?? "MSG";
    public string BodyPreview => string.IsNullOrWhiteSpace(Body) ? "(no content)" : Body;
}
