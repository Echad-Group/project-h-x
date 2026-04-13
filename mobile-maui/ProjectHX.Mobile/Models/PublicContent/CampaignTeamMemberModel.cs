namespace ProjectHX.Mobile.Models.PublicContent;

public sealed class CampaignTeamMemberModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public bool HasPhoto => !string.IsNullOrWhiteSpace(PhotoUrl);

    public string Initials
    {
        get
        {
            var initials = string.Join(string.Empty,
                Name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Take(2)
                    .Select(part => char.ToUpperInvariant(part[0])));

            return string.IsNullOrWhiteSpace(initials) ? "?" : initials;
        }
    }
}
