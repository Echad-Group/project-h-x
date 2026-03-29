namespace ProjectHX.Mobile.Models.PublicContent;

public sealed class IssueInitiativeModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
