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

    public class ComplianceReminderRequest
    {
        public string? UserId { get; set; }
        public string Channel { get; set; } = CampaignMessageChannels.InApp;
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
    }
}