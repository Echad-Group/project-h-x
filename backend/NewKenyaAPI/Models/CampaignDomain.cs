using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models
{
    public static class CampaignVerificationStatuses
    {
        public const string Pending = "Pending";
        public const string Verified = "Verified";
        public const string Rejected = "Rejected";
    }

    public static class CampaignVoterCardStatuses
    {
        public const string Missing = "Missing";
        public const string Pending = "Pending";
        public const string Verified = "Verified";
        public const string Rejected = "Rejected";
    }

    public static class CampaignTaskStatuses
    {
        public const string Pending = "Pending";
        public const string InProgress = "In Progress";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
    }

    public static class CampaignMessageChannels
    {
        public const string WhatsApp = "WhatsApp";
        public const string Push = "Push";
        public const string InApp = "InApp";
    }

    public static class CampaignMessageStatuses
    {
        public const string Queued = "Queued";
        public const string Processing = "Processing";
        public const string Sent = "Sent";
        public const string Delivered = "Delivered";
        public const string Read = "Read";
        public const string Failed = "Failed";
    }

    public static class OtpPurposes
    {
        public const string Login = "Login";
        public const string Registration = "Registration";
        public const string SensitiveAction = "SensitiveAction";
    }

    public static class ElectionResultStatuses
    {
        public const string PendingValidation = "PendingValidation";
        public const string Validated = "Validated";
        public const string Rejected = "Rejected";
    }

    public class CampaignMessage
    {
        public int Id { get; set; }

        [Required]
        public string SenderUserId { get; set; } = string.Empty;
        public ApplicationUser? SenderUser { get; set; }

        [Required]
        public string ReceiverUserId { get; set; } = string.Empty;
        public ApplicationUser? ReceiverUser { get; set; }

        [Required]
        [MaxLength(20)]
        public string Channel { get; set; } = CampaignMessageChannels.InApp;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = CampaignMessageStatuses.Queued;

        [Required]
        [MaxLength(1000)]
        public string Body { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Url { get; set; }

        [MaxLength(100)]
        public string? EventId { get; set; }

        public int RetryCount { get; set; }
        public int MaxRetries { get; set; } = 3;
        public DateTime? NextAttemptAt { get; set; }

        [MaxLength(1000)]
        public string? LastError { get; set; }

        [MaxLength(1000)]
        public string? DeadLetterReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    public class ComplianceReminder
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required]
        [MaxLength(50)]
        public string ReminderType { get; set; } = "VoterCard";

        [MaxLength(20)]
        public string Channel { get; set; } = CampaignMessageChannels.InApp;

        [MaxLength(20)]
        public string Status { get; set; } = CampaignMessageStatuses.Queued;

        public int EscalationLevel { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public string? Notes { get; set; }
    }

    public class LeaderboardScore
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int TotalPoints { get; set; }
        public int DirectDownlines { get; set; }
        public int TasksCompleted { get; set; }
        public int VerifiedVoterCards { get; set; }
        public int VerificationIntegrityPoints { get; set; }

        [MaxLength(80)]
        public string BadgeTier { get; set; } = "Bronze";

        [MaxLength(120)]
        public string RecognitionTitle { get; set; } = "Field Contributor";

        [MaxLength(160)]
        public string IncentiveTag { get; set; } = "Weekly Recognition";

        [MaxLength(50)]
        public string Scope { get; set; } = "National";

        [MaxLength(100)]
        public string? Region { get; set; }

        [MaxLength(100)]
        public string? County { get; set; }

        public DateTime LastCalculatedAt { get; set; } = DateTime.UtcNow;
    }

    public class OtpVerificationCode
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required]
        [MaxLength(20)]
        public string Purpose { get; set; } = OtpPurposes.Login;

        [Required]
        [MaxLength(200)]
        public string CodeHash { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public int Attempts { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UsedAt { get; set; }
    }

    public class AddDownlineRequest
    {
        [Required]
        public string LeaderUserId { get; set; } = string.Empty;

        [Required]
        public string DownlineUserId { get; set; } = string.Empty;
    }

    public class ReassignDownlineRequest
    {
        [Required]
        public string CurrentLeaderUserId { get; set; } = string.Empty;

        [Required]
        public string NewLeaderUserId { get; set; } = string.Empty;

        [Required]
        public string DownlineUserId { get; set; } = string.Empty;
    }

    public class RemoveDownlineRequest
    {
        [Required]
        public string LeaderUserId { get; set; } = string.Empty;

        [Required]
        public string DownlineUserId { get; set; } = string.Empty;
    }

    public class CampaignTaskCreateRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Deadline { get; set; }

        [MaxLength(50)]
        public string Priority { get; set; } = "Medium";

        public string? AssignedToUserId { get; set; }
        public string? Location { get; set; }
        public string? Region { get; set; }
        public string? County { get; set; }
        public string? SubCounty { get; set; }
        public string? Constituency { get; set; }
        public string? Ward { get; set; }
        public string? PollingStation { get; set; }
    }

    public class CampaignTaskAssignRequest
    {
        [Required]
        public int TaskId { get; set; }

        [Required]
        public string AssignedToUserId { get; set; } = string.Empty;
    }

    public class CampaignTaskStatusUpdateRequest
    {
        [Required]
        public int TaskId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = CampaignTaskStatuses.InProgress;
    }

    public class CampaignTaskCompleteRequest
    {
        [Required]
        public int TaskId { get; set; }

        [MaxLength(1000)]
        public string? CompletionNotes { get; set; }
    }

    public class CampaignTaskUpdateRequest
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(50)]
        public string? Priority { get; set; }

        public DateTime? Deadline { get; set; }
        public string? AssignedToUserId { get; set; }
        public string? Location { get; set; }
        public string? Region { get; set; }
        public string? County { get; set; }
        public string? SubCounty { get; set; }
        public string? Constituency { get; set; }
        public string? Ward { get; set; }
        public string? PollingStation { get; set; }
    }

    public class ComplianceReminderRequest
    {
        public string? UserId { get; set; }
        public string Channel { get; set; } = CampaignMessageChannels.InApp;
        public bool DualChannel { get; set; }
        public string TemplateKey { get; set; } = "DailyVoterCardReminder";
        public bool DryRun { get; set; }
    }

    public class SendOtpRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Purpose { get; set; } = OtpPurposes.Login;
    }

    public class VerifyOtpRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Purpose { get; set; } = OtpPurposes.Login;

        [MaxLength(20)]
        public string Level { get; set; } = "phone";
    }

    public class MessageBroadcastRequest
    {
        [Required]
        [MaxLength(20)]
        public string Channel { get; set; } = CampaignMessageChannels.InApp;

        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Body { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Url { get; set; }

        public DateTime? ScheduledFor { get; set; }
    }

    public class MessageTargetRequest : MessageBroadcastRequest
    {
        public List<string>? ReceiverUserIds { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }

        [MaxLength(100)]
        public string? County { get; set; }

        [MaxLength(50)]
        public string? CampaignRole { get; set; }
    }

    public class ElectionResult
    {
        public int Id { get; set; }

        [Required]
        public string SubmittedByUserId { get; set; } = string.Empty;
        public ApplicationUser? SubmittedByUser { get; set; }

        [Required]
        [MaxLength(120)]
        public string PollingStationCode { get; set; } = string.Empty;

        [MaxLength(120)]
        public string? Constituency { get; set; }

        [MaxLength(120)]
        public string? County { get; set; }

        [MaxLength(120)]
        public string? Region { get; set; }

        public int CandidateA { get; set; }
        public int CandidateB { get; set; }
        public int CandidateC { get; set; }
        public int RejectedVotes { get; set; }
        public int RegisteredVoters { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = ElectionResultStatuses.PendingValidation;

        [MaxLength(1000)]
        public string? ValidationNotes { get; set; }

        [MaxLength(450)]
        public string? ReviewedByUserId { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public bool IsConflictFlagged { get; set; }

        [MaxLength(160)]
        public string? ConflictGroupKey { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        [MaxLength(160)]
        public string? DeviceFingerprint { get; set; }

        public decimal? IntegrityConfidenceScore { get; set; }
        public bool IsTamperSuspected { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ValidatedAt { get; set; }

        [MaxLength(80)]
        public string ReportingWindow { get; set; } = "";
    }

    public class ElectionResultSubmitRequest
    {
        [Required]
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
    }

    public class ElectionResultReviewRequest
    {
        [Required]
        [MaxLength(20)]
        public string Decision { get; set; } = "Validated";

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class ElectionConflictAdjudicationRequest
    {
        [Required]
        public int AcceptedResultId { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class CampaignGeoPing
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal? AccuracyMeters { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }

        [MaxLength(100)]
        public string? County { get; set; }

        [MaxLength(120)]
        public string Purpose { get; set; } = "Coverage";

        public bool ConsentGranted { get; set; }
        public bool IsAnonymized { get; set; }
        public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
    }

    public class CampaignGeoPingRequest
    {
        [Range(-90, 90)]
        public decimal Latitude { get; set; }

        [Range(-180, 180)]
        public decimal Longitude { get; set; }

        [Range(0, 50000)]
        public decimal? AccuracyMeters { get; set; }

        [MaxLength(120)]
        public string Purpose { get; set; } = "Coverage";

        public bool ConsentGranted { get; set; }
        public bool IsAnonymized { get; set; }
    }

    public class AuditLogEvent
    {
        public long Id { get; set; }

        [MaxLength(450)]
        public string? ActorUserId { get; set; }

        [Required]
        [MaxLength(80)]
        public string EventType { get; set; } = string.Empty;

        [MaxLength(80)]
        public string? ResourceType { get; set; }

        [MaxLength(160)]
        public string? ResourceId { get; set; }

        [MaxLength(3000)]
        public string? DetailsJson { get; set; }

        [MaxLength(80)]
        public string? Source { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class WarRoomStateResponse
    {
        public string WarRoomName { get; set; } = string.Empty;
        public DateTime SnapshotAt { get; set; }
        public List<WarRoomCommandLane> Lanes { get; set; } = new();
        public List<WarRoomIncidentItem> ActiveIncidents { get; set; } = new();
        public List<WarRoomCommandPodItem> CommandPods { get; set; } = new();
    }

    public class PagedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class WarRoomCommandPodItem
    {
        [Required]
        [MaxLength(60)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [MaxLength(160)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(40)]
        public string Status { get; set; } = "Active";

        [Required]
        [MaxLength(120)]
        public string BattlegroundRegion { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string DemographicBloc { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string OpponentLane { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? FocusObjective { get; set; }

        public List<WarRoomCommandPodMember> Members { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
    }

    public class WarRoomCommandPodMember
    {
        [Required]
        [MaxLength(120)]
        public string Role { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string LeadName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Lane { get; set; } = string.Empty;
    }

    public class WarRoomCommandPodCreateRequest
    {
        [Required]
        [MaxLength(160)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string BattlegroundRegion { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string DemographicBloc { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string OpponentLane { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? FocusObjective { get; set; }
    }

    public class WarRoomCommandPodUpdateRequest
    {
        [MaxLength(40)]
        public string? Status { get; set; }

        [MaxLength(120)]
        public string? BattlegroundRegion { get; set; }

        [MaxLength(120)]
        public string? DemographicBloc { get; set; }

        [MaxLength(120)]
        public string? OpponentLane { get; set; }

        [MaxLength(300)]
        public string? FocusObjective { get; set; }
    }

    public class WarRoomRedZoneState
    {
        public bool IsElectionWeekModeEnabled { get; set; }
        public DateTime? ActivatedAt { get; set; }

        [MaxLength(450)]
        public string? ActivatedByUserId { get; set; }

        [Range(15, 180)]
        public int DecisionIntervalMinutes { get; set; } = 60;

        public DateTime? LastDecisionAt { get; set; }
        public DateTime? NextCheckpointAt { get; set; }
        public List<WarRoomRedZoneDecisionLogItem> Decisions { get; set; } = new();
    }

    public class WarRoomRedZoneDecisionLogItem
    {
        public int Id { get; set; }
        public DateTime LoggedAt { get; set; }

        [Required]
        [MaxLength(200)]
        public string DecisionTitle { get; set; } = string.Empty;

        [Required]
        [MaxLength(1200)]
        public string DecisionSummary { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string CheckpointHourLabel { get; set; } = string.Empty;

        [MaxLength(120)]
        public string? OwnerRole { get; set; }

        [MaxLength(20)]
        public string Severity { get; set; } = "Medium";

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class WarRoomRedZoneToggleRequest
    {
        public bool Enable { get; set; }

        [Range(15, 180)]
        public int DecisionIntervalMinutes { get; set; } = 60;
    }

    public class WarRoomRedZoneDecisionCreateRequest
    {
        [Required]
        [MaxLength(200)]
        public string DecisionTitle { get; set; } = string.Empty;

        [Required]
        [MaxLength(1200)]
        public string DecisionSummary { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string CheckpointHourLabel { get; set; } = string.Empty;

        [MaxLength(120)]
        public string? OwnerRole { get; set; }

        [MaxLength(20)]
        public string Severity { get; set; } = "Medium";

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class WarRoomCommandGridStatusItem
    {
        [Required]
        [MaxLength(80)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [MaxLength(40)]
        public string Tier { get; set; } = string.Empty;

        [Required]
        [MaxLength(160)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(80)]
        public string? ParentId { get; set; }

        [Required]
        [MaxLength(30)]
        public string ReadinessStatus { get; set; } = "Ready";

        [Range(0, 100)]
        public int StaffingPercent { get; set; }

        [Required]
        [MaxLength(40)]
        public string ConnectivityStatus { get; set; } = "Online";

        public int ActiveIncidents { get; set; }
        public DateTime LastHeartbeatAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class WarRoomCommandGridUpdateRequest
    {
        [MaxLength(30)]
        public string? ReadinessStatus { get; set; }

        [Range(0, 100)]
        public int? StaffingPercent { get; set; }

        [MaxLength(40)]
        public string? ConnectivityStatus { get; set; }

        [Range(0, 10000)]
        public int? ActiveIncidents { get; set; }
    }

    public class CoalitionCommandItem
    {
        [Required]
        [MaxLength(80)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [MaxLength(160)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string DirectorName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        public List<CoalitionOutreachModuleItem> Modules { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
    }

    public class CoalitionOutreachModuleItem
    {
        [Required]
        [MaxLength(40)]
        public string GroupType { get; set; } = string.Empty;

        [Required]
        [MaxLength(180)]
        public string Objective { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Planned";

        [MaxLength(300)]
        public string? ExecutionPlan { get; set; }

        public DateTime? LastEngagementAt { get; set; }
    }

    public class CoalitionCommandCreateRequest
    {
        [Required]
        [MaxLength(160)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string DirectorName { get; set; } = string.Empty;
    }

    public class CoalitionModuleUpdateRequest
    {
        [Required]
        [MaxLength(40)]
        public string GroupType { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(300)]
        public string? ExecutionPlan { get; set; }
    }

    public class MobilizationRoleAssignmentItem
    {
        [Required]
        [MaxLength(60)]
        public string RoleCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(160)]
        public string RoleTitle { get; set; } = string.Empty;

        [MaxLength(120)]
        public string? AssignedLeadName { get; set; }

        [MaxLength(120)]
        public string? RegionFocus { get; set; }

        [Required]
        [MaxLength(40)]
        public string Status { get; set; } = "Open";

        [MaxLength(300)]
        public string? OperationalNotes { get; set; }
    }

    public class MobilizationRoleAssignmentUpdateRequest
    {
        [MaxLength(120)]
        public string? AssignedLeadName { get; set; }

        [MaxLength(120)]
        public string? RegionFocus { get; set; }

        [MaxLength(40)]
        public string? Status { get; set; }

        [MaxLength(300)]
        public string? OperationalNotes { get; set; }
    }

    public class CampaignPhaseState
    {
        [Required]
        [MaxLength(40)]
        public string ActivePhase { get; set; } = "Exploratory";

        public DateTime ActiveSince { get; set; }

        [MaxLength(450)]
        public string? UpdatedByUserId { get; set; }

        public List<CampaignPhasePlanItem> Phases { get; set; } = new();
    }

    public class CampaignPhasePlanItem
    {
        [Required]
        [MaxLength(40)]
        public string Phase { get; set; } = string.Empty;

        [Required]
        [MaxLength(40)]
        public string Status { get; set; } = "Inactive";

        [MaxLength(300)]
        public string? PlaybookSummary { get; set; }

        public List<CampaignPhaseKpiTarget> Kpis { get; set; } = new();
    }

    public class CampaignPhaseKpiTarget
    {
        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string TargetValue { get; set; } = string.Empty;
    }

    public class CampaignPhaseSwitchRequest
    {
        [Required]
        [MaxLength(40)]
        public string Phase { get; set; } = string.Empty;
    }

    public class VerificationQueueItem
    {
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(120)]
        public string? FullName { get; set; }

        [MaxLength(40)]
        public string VerificationStatus { get; set; } = CampaignVerificationStatuses.Pending;

        [MaxLength(40)]
        public string VoterCardStatus { get; set; } = CampaignVoterCardStatuses.Pending;

        [MaxLength(40)]
        public string? CampaignRole { get; set; }

        [MaxLength(120)]
        public string? Region { get; set; }

        [MaxLength(120)]
        public string? County { get; set; }

        [MaxLength(300)]
        public string? NidaDocumentUrl { get; set; }

        [MaxLength(300)]
        public string? VoterCardDocumentUrl { get; set; }

        [MaxLength(300)]
        public string? SelfieDocumentUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public List<VerificationTimelineEvent> Timeline { get; set; } = new();
    }

    public class VerificationTimelineEvent
    {
        public DateTime Timestamp { get; set; }

        [Required]
        [MaxLength(80)]
        public string Action { get; set; } = string.Empty;

        [MaxLength(120)]
        public string? ReviewerName { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class VerificationDecisionRequest
    {
        [Required]
        [MaxLength(40)]
        public string Decision { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? ReviewerNotes { get; set; }
    }

    public class WarRoomCommandLane
    {
        [Required]
        [MaxLength(50)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Operational";

        [Required]
        [MaxLength(120)]
        public string Lead { get; set; } = string.Empty;

        public int Priority { get; set; }
        public int OpenIncidents { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class WarRoomIncidentItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Severity { get; set; } = "Medium";

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Open";

        [Required]
        [MaxLength(50)]
        public string Lane { get; set; } = string.Empty;

        [MaxLength(450)]
        public string? OwnerUserId { get; set; }

        [MaxLength(1000)]
        public string? ResolutionNotes { get; set; }

        public int EscalationLevel { get; set; }

        [MaxLength(120)]
        public string? EscalationOwnerRole { get; set; }

        public DateTime? EscalationDueAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class WarRoomIncidentCreateRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Severity { get; set; } = "Medium";

        [MaxLength(50)]
        public string Lane { get; set; } = "strategy";
    }

    public class WarRoomIncidentUpdateRequest
    {
        [MaxLength(20)]
        public string? Status { get; set; }

        [MaxLength(450)]
        public string? OwnerUserId { get; set; }

        [MaxLength(1000)]
        public string? ResolutionNotes { get; set; }
    }

    public class WarRoomIncidentEscalateRequest
    {
        [Range(1, 4)]
        public int EscalationLevel { get; set; }

        [MaxLength(1000)]
        public string? Rationale { get; set; }
    }

    public class WarRoomBattleRhythmItem
    {
        [Required]
        [MaxLength(80)]
        public string Id { get; set; } = string.Empty;

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        [MaxLength(10)]
        public string TimeSlot { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Purpose { get; set; }

        public bool Completed { get; set; }
        public DateTime? CompletedAt { get; set; }

        [MaxLength(450)]
        public string? CompletedByUserId { get; set; }

        public int AttendanceCount { get; set; }

        [MaxLength(1000)]
        public string? CompletionNotes { get; set; }
    }

    public class WarRoomBattleRhythmCompleteRequest
    {
        [Range(0, 500)]
        public int AttendanceCount { get; set; }

        [MaxLength(1000)]
        public string? CompletionNotes { get; set; }
    }

    public class WarRoomLegalCaseItem
    {
        public int Id { get; set; }

        public int? IncidentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string Jurisdiction { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string FilingType { get; set; } = string.Empty;

        [Required]
        [MaxLength(40)]
        public string Status { get; set; } = "Open";

        [MaxLength(450)]
        public string? AssignedLawyerUserId { get; set; }

        [MaxLength(2000)]
        public string? Summary { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class WarRoomLegalCaseCreateRequest
    {
        public int? IncidentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string Jurisdiction { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string FilingType { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Summary { get; set; }
    }

    public class WarRoomLegalCaseUpdateRequest
    {
        [MaxLength(40)]
        public string? Status { get; set; }

        [MaxLength(450)]
        public string? AssignedLawyerUserId { get; set; }

        [MaxLength(2000)]
        public string? Summary { get; set; }
    }
}