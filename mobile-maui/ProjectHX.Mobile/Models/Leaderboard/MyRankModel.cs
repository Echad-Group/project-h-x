namespace ProjectHX.Mobile.Models.Leaderboard;

public sealed class MyRankModel
{
    public int Rank { get; set; }
    public int TotalParticipants { get; set; }
    public int TotalPoints { get; set; }
    public string? BadgeTier { get; set; }
    public string? RecognitionTitle { get; set; }
    public string? IncentiveTag { get; set; }
    public string? Region { get; set; }
    public string? County { get; set; }

    public string RankDisplay => $"#{Rank} of {TotalParticipants}";
    public string PointsDisplay => $"{TotalPoints:N0} pts";
    public string LocationDisplay
    {
        get
        {
            var parts = new[] { County, Region }.Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(", ", parts);
        }
    }
}
