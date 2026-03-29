namespace ProjectHX.Mobile.Models.Volunteer;

public sealed class VolunteerStatusResponse
{
    public bool IsVolunteer { get; set; }
    public VolunteerProfileModel? Volunteer { get; set; }
}
