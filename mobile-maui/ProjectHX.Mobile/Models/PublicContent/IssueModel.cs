namespace ProjectHX.Mobile.Models.PublicContent;

public sealed class IssueModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public List<IssueInitiativeModel> Initiatives { get; set; } = [];
    public List<IssueQuestionModel> Questions { get; set; } = [];
}
