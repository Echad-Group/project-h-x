namespace ProjectHX.Mobile.Models.Results;

public sealed class ResultSubmissionRequest
{
    public string PollingStationCode { get; set; } = string.Empty;
    public string? Constituency { get; set; }
    public string? County { get; set; }
    public string? Region { get; set; }
    public int CandidateA { get; set; }
    public int CandidateB { get; set; }
    public int CandidateC { get; set; }
    public int RejectedVotes { get; set; }
    public int RegisteredVoters { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? DeviceFingerprint { get; set; }
    public string? EvidenceFilePath { get; set; }
}
