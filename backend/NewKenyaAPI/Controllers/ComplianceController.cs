using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using System.Security.Claims;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.LeadershipAccess)]
    public class ComplianceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ComplianceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetSummary()
        {
            var totalUsers = await _context.Users.CountAsync();
            var missingVoterCards = await _context.Users.CountAsync(user => user.VoterCardStatus == CampaignVoterCardStatuses.Missing);
            var pendingVoterCards = await _context.Users.CountAsync(user => user.VoterCardStatus == CampaignVoterCardStatuses.Pending);
            var verifiedVoterCards = await _context.Users.CountAsync(user => user.VoterCardStatus == CampaignVoterCardStatuses.Verified);
            var day3Escalations = await _context.Users.CountAsync(user => user.VoterCardStatus == CampaignVoterCardStatuses.Missing && user.CreatedAt <= DateTime.UtcNow.AddDays(-3));
            var day7Escalations = await _context.Users.CountAsync(user => user.VoterCardStatus == CampaignVoterCardStatuses.Missing && user.CreatedAt <= DateTime.UtcNow.AddDays(-7));

            var recentReminders = await _context.ComplianceReminders
                .Include(reminder => reminder.User)
                .OrderByDescending(reminder => reminder.CreatedAt)
                .Take(10)
                .Select(reminder => new
                {
                    reminder.Id,
                    reminder.UserId,
                    fullName = BuildFullName(reminder.User),
                    reminder.Channel,
                    reminder.Status,
                    reminder.EscalationLevel,
                    reminder.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                totalUsers,
                missingVoterCards,
                pendingVoterCards,
                verifiedVoterCards,
                day3Escalations,
                day7Escalations,
                recentReminders
            });
        }

        [HttpPost("reminder")]
        public async Task<ActionResult<object>> QueueReminder([FromBody] ComplianceReminderRequest request)
        {
            var senderUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(senderUserId))
            {
                return Unauthorized();
            }

            var query = _context.Users.Where(user => user.VoterCardStatus == CampaignVoterCardStatuses.Missing);

            if (!string.IsNullOrWhiteSpace(request.UserId))
            {
                query = query.Where(user => user.Id == request.UserId);
            }

            var users = await query.ToListAsync();
            if (users.Count == 0)
            {
                return Ok(new { message = "No users require a voter-card reminder.", queuedCount = 0 });
            }

            var queuedReminders = new List<ComplianceReminder>();
            var queuedMessages = new List<CampaignMessage>();
            foreach (var user in users)
            {
                var escalationLevel = ResolveEscalationLevel(user.CreatedAt);
                var templateBody = ResolveTemplateBody(request.TemplateKey, user, escalationLevel);

                queuedReminders.Add(new ComplianceReminder
                {
                    UserId = user.Id,
                    Channel = request.Channel,
                    Status = CampaignMessageStatuses.Queued,
                    EscalationLevel = escalationLevel,
                    Notes = templateBody,
                    CreatedAt = DateTime.UtcNow
                });

                queuedMessages.Add(new CampaignMessage
                {
                    SenderUserId = senderUserId,
                    ReceiverUserId = user.Id,
                    Channel = request.Channel,
                    Title = "Voter Card Compliance Reminder",
                    Body = templateBody,
                    Status = CampaignMessageStatuses.Queued,
                    NextAttemptAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                });

                if (request.DualChannel && !string.Equals(request.Channel, CampaignMessageChannels.WhatsApp, StringComparison.OrdinalIgnoreCase))
                {
                    queuedReminders.Add(new ComplianceReminder
                    {
                        UserId = user.Id,
                        Channel = CampaignMessageChannels.WhatsApp,
                        Status = CampaignMessageStatuses.Queued,
                        EscalationLevel = escalationLevel,
                        Notes = templateBody,
                        CreatedAt = DateTime.UtcNow
                    });

                    queuedMessages.Add(new CampaignMessage
                    {
                        SenderUserId = senderUserId,
                        ReceiverUserId = user.Id,
                        Channel = CampaignMessageChannels.WhatsApp,
                        Title = "Voter Card Compliance Reminder",
                        Body = templateBody,
                        Status = CampaignMessageStatuses.Queued,
                        NextAttemptAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (!request.DryRun)
            {
                _context.ComplianceReminders.AddRange(queuedReminders);
                _context.CampaignMessages.AddRange(queuedMessages);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                message = request.DryRun ? "Dry run completed." : "Compliance reminders queued.",
                queuedCount = queuedReminders.Count,
                templateKey = request.TemplateKey,
                dualChannel = request.DualChannel,
                users = users.Select(user => new
                {
                    user.Id,
                    fullName = BuildFullName(user),
                    escalationLevel = ResolveEscalationLevel(user.CreatedAt)
                })
            });
        }

        private static string ResolveTemplateBody(string? templateKey, ApplicationUser user, int escalationLevel)
        {
            var fullName = BuildFullName(user);
            return (templateKey ?? string.Empty).Trim().ToLowerInvariant() switch
            {
                "nearestoffice" => $"{fullName}, please complete voter-card verification today. Visit your nearest IEC office for support.",
                "day3escalation" => $"{fullName}, this is a day-3 compliance reminder. Please submit your voter-card proof immediately to avoid escalation.",
                "day7escalation" => $"{fullName}, this is a day-7 final notice. Your compliance status is escalated and requires urgent action today.",
                _ => escalationLevel switch
                {
                    >= 2 => $"{fullName}, final reminder: submit your voter-card proof now. Case is at escalation level {escalationLevel}.",
                    1 => $"{fullName}, reminder: please submit voter-card proof. Current escalation level {escalationLevel}.",
                    _ => $"{fullName}, daily reminder: upload your voter-card proof to keep your campaign account compliant."
                }
            };
        }

        private static int ResolveEscalationLevel(DateTime createdAt)
        {
            var ageInDays = (DateTime.UtcNow.Date - createdAt.Date).Days;
            if (ageInDays >= 7)
            {
                return 2;
            }

            if (ageInDays >= 3)
            {
                return 1;
            }

            return 0;
        }

        private static string BuildFullName(ApplicationUser? user)
        {
            if (user == null)
            {
                return "Unknown User";
            }

            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            return string.IsNullOrWhiteSpace(fullName) ? user.Email ?? user.UserName ?? "Unknown User" : fullName;
        }
    }
}