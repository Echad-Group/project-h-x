namespace ProjectHX.Mobile.Models.PublicContent;

public sealed class EventRsvpRequestModel
{
    public int EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int NumberOfGuests { get; set; } = 1;
    public string? SpecialRequirements { get; set; }
}
