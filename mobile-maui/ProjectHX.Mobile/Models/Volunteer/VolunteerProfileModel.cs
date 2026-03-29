namespace ProjectHX.Mobile.Models.Volunteer;

public sealed class VolunteerProfileModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? AvailabilityZones { get; set; }
    public string? Skills { get; set; }
    public int? HoursPerWeek { get; set; }
    public bool AvailableWeekends { get; set; }
    public bool AvailableEvenings { get; set; }
    public string? Interests { get; set; }
    public DateTime CreatedAt { get; set; }
}
