namespace ProjectHX.Mobile.Models.Tasks;

public sealed class TaskCompleteRequest
{
    public int TaskId { get; set; }
    public string? CompletionNotes { get; set; }
}
