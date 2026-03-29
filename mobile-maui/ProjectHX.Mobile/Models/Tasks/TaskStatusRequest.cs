namespace ProjectHX.Mobile.Models.Tasks;

public sealed class TaskStatusRequest
{
    public int TaskId { get; set; }
    public string Status { get; set; } = "InProgress";
}
