namespace ProjectHX.Mobile.Models.PublicContent;

public sealed class CampaignEventModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Location { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? ImageUrl { get; set; }
    public string? Type { get; set; }
    public int? Capacity { get; set; }

    public string DateDisplay => Date.ToLocalTime().ToString("ddd d MMM, HH:mm");
}
