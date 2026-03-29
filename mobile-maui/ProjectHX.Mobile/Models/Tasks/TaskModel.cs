namespace ProjectHX.Mobile.Models.Tasks;

public sealed class TaskModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public string? Region { get; set; }
    public string? County { get; set; }
    public string? Constituency { get; set; }
    public string? Ward { get; set; }
    public string? PollingStation { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Computed helpers for the UI
    public bool IsPending => Status == "Pending";
    public bool IsInProgress => Status == "InProgress";
    public bool IsCompleted => Status == "Completed";
    public bool CanStart => IsPending;
    public bool CanComplete => IsInProgress;

    public string DeadlineDisplay => Deadline.HasValue
        ? Deadline.Value.ToLocalTime().ToString("ddd d MMM, HH:mm")
        : "No deadline";

    public string StatusDisplay => Status switch
    {
        "Pending" => "Pending",
        "InProgress" => "In Progress",
        "Completed" => "Completed",
        _ => Status
    };

    public Color StatusColor => Status switch
    {
        "Pending" => Color.FromArgb("#B45309"),
        "InProgress" => Color.FromArgb("#0369A1"),
        "Completed" => Color.FromArgb("#065F46"),
        _ => Colors.Gray
    };

    public string LocationSummary
    {
        get
        {
            var parts = new[] { Ward, Constituency, County, Region }
                .Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(" · ", parts);
        }
    }
}
