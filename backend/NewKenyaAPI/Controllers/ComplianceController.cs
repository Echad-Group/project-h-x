using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;

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

            var queuedReminders = users.Select(user => new ComplianceReminder
            {
                UserId = user.Id,
                Channel = request.Channel,
                Status = CampaignMessageStatuses.Queued,
                EscalationLevel = ResolveEscalationLevel(user.CreatedAt),
                Notes = "Queued by compliance reminder endpoint.",
                CreatedAt = DateTime.UtcNow
            }).ToList();

            if (!request.DryRun)
            {
                _context.ComplianceReminders.AddRange(queuedReminders);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                message = request.DryRun ? "Dry run completed." : "Compliance reminders queued.",
                queuedCount = queuedReminders.Count,
                users = users.Select(user => new
                {
                    user.Id,
                    fullName = BuildFullName(user),
                    escalationLevel = ResolveEscalationLevel(user.CreatedAt)
                })
            });
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