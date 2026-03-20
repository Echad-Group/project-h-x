using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using Task = System.Threading.Tasks.Task;

namespace NewKenyaAPI.Services
{
    public class ComplianceReminderScheduler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ComplianceReminderScheduler> _logger;
        private DateOnly? _lastRunDate;

        public ComplianceReminderScheduler(IServiceProvider serviceProvider, ILogger<ComplianceReminderScheduler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunDailyAutomationAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Compliance reminder scheduler failed");
                }

                await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
            }
        }

        private async Task RunDailyAutomationAsync(CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (_lastRunDate.HasValue && _lastRunDate.Value == today)
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var systemSender = await ResolveSystemSenderAsync(context, cancellationToken);
            if (string.IsNullOrWhiteSpace(systemSender))
            {
                _logger.LogWarning("Compliance scheduler skipped: no sender user available.");
                return;
            }

            var users = await context.Users
                .Where(user => user.VoterCardStatus == CampaignVoterCardStatuses.Missing)
                .ToListAsync(cancellationToken);

            if (users.Count == 0)
            {
                _lastRunDate = today;
                return;
            }

            var reminders = new List<ComplianceReminder>();
            var messages = new List<CampaignMessage>();

            foreach (var user in users)
            {
                var level = ResolveEscalationLevel(user.CreatedAt);
                var nearestOffice = ResolveNearestIecOffice(user.County, user.Region);
                var body = BuildPersonalizedReminderBody(user, nearestOffice, level);

                reminders.Add(new ComplianceReminder
                {
                    UserId = user.Id,
                    Channel = CampaignMessageChannels.InApp,
                    ReminderType = "VoterCard",
                    Status = CampaignMessageStatuses.Queued,
                    EscalationLevel = level,
                    Notes = body,
                    CreatedAt = DateTime.UtcNow
                });

                reminders.Add(new ComplianceReminder
                {
                    UserId = user.Id,
                    Channel = CampaignMessageChannels.WhatsApp,
                    ReminderType = "VoterCard",
                    Status = CampaignMessageStatuses.Queued,
                    EscalationLevel = level,
                    Notes = body,
                    CreatedAt = DateTime.UtcNow
                });

                messages.Add(new CampaignMessage
                {
                    SenderUserId = systemSender,
                    ReceiverUserId = user.Id,
                    Channel = CampaignMessageChannels.InApp,
                    Title = "Daily Voter Card Reminder",
                    Body = body,
                    Status = CampaignMessageStatuses.Queued,
                    NextAttemptAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                });

                messages.Add(new CampaignMessage
                {
                    SenderUserId = systemSender,
                    ReceiverUserId = user.Id,
                    Channel = CampaignMessageChannels.WhatsApp,
                    Title = "Daily Voter Card Reminder",
                    Body = body,
                    Status = CampaignMessageStatuses.Queued,
                    NextAttemptAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                });

                if (level >= 1)
                {
                    var leaders = await ResolveFollowupLeadersAsync(context, user, cancellationToken);
                    foreach (var leader in leaders)
                    {
                        messages.Add(new CampaignMessage
                        {
                            SenderUserId = systemSender,
                            ReceiverUserId = leader.Id,
                            Channel = CampaignMessageChannels.InApp,
                            Title = $"Escalation D{(level == 1 ? 3 : 7)} follow-up required",
                            Body = $"Member {BuildFullName(user)} requires voter-card compliance intervention (level {level}).",
                            Status = CampaignMessageStatuses.Queued,
                            NextAttemptAt = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            context.ComplianceReminders.AddRange(reminders);
            context.CampaignMessages.AddRange(messages);
            await context.SaveChangesAsync(cancellationToken);

            _lastRunDate = today;
            _logger.LogInformation("Compliance scheduler queued {ReminderCount} reminders and {MessageCount} messages", reminders.Count, messages.Count);
        }

        private static int ResolveEscalationLevel(DateTime createdAt)
        {
            var ageDays = (DateTime.UtcNow.Date - createdAt.Date).Days;
            if (ageDays >= 7)
            {
                return 2;
            }

            if (ageDays >= 3)
            {
                return 1;
            }

            return 0;
        }

        private static string BuildPersonalizedReminderBody(ApplicationUser user, string nearestOffice, int escalationLevel)
        {
            var fullName = BuildFullName(user);
            return escalationLevel switch
            {
                >= 2 => $"{fullName}, final escalation: submit voter-card proof today. Nearest IEC office: {nearestOffice}.",
                1 => $"{fullName}, day-3 escalation: your voter-card status is still missing. Nearest IEC office: {nearestOffice}.",
                _ => $"{fullName}, daily reminder: please complete voter-card verification. Nearest IEC office: {nearestOffice}."
            };
        }

        private static string ResolveNearestIecOffice(string? county, string? region)
        {
            var key = (county ?? region ?? string.Empty).Trim().ToLowerInvariant();
            return key switch
            {
                "nairobi" => "Nairobi Huduma Centre - City Square",
                "nakuru" => "Nakuru Huduma Centre - Government Road",
                "mombasa" => "Mombasa Huduma Centre - Treasury Square",
                _ => "Nearest county IEC service desk"
            };
        }

        private static async Task<List<ApplicationUser>> ResolveFollowupLeadersAsync(ApplicationDbContext context, ApplicationUser user, CancellationToken cancellationToken)
        {
            return await context.Users
                .Where(candidate => candidate.County == user.County
                    && (candidate.CampaignRole == UserRoles.CountyLeader
                        || candidate.CampaignRole == UserRoles.RegionalLeader
                        || candidate.CampaignRole == UserRoles.SubCountyLeader))
                .Take(5)
                .ToListAsync(cancellationToken);
        }

        private static async Task<string?> ResolveSystemSenderAsync(ApplicationDbContext context, CancellationToken cancellationToken)
        {
            var admin = await context.Users.FirstOrDefaultAsync(user => user.CampaignRole == UserRoles.Admin, cancellationToken);
            if (admin != null)
            {
                return admin.Id;
            }

            var anyLeader = await context.Users.FirstOrDefaultAsync(user => user.CampaignRole == UserRoles.CountyLeader || user.CampaignRole == UserRoles.RegionalLeader, cancellationToken);
            return anyLeader?.Id;
        }

        private static string BuildFullName(ApplicationUser user)
        {
            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            return string.IsNullOrWhiteSpace(fullName) ? user.Email ?? user.UserName ?? "Member" : fullName;
        }
    }
}
