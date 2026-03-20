using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Controllers
{
    [Route("api/ops-health")]
    [ApiController]
    [Authorize(Roles = UserRoles.LeadershipAccess)]
    public class OperationsHealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OperationsHealthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetHealth()
        {
            var now = DateTime.UtcNow;
            var lastHour = now.AddHours(-1);

            var queuedMessages = await _context.CampaignMessages.CountAsync(message => message.Status == CampaignMessageStatuses.Queued);
            var failedMessages = await _context.CampaignMessages.CountAsync(message => message.Status == CampaignMessageStatuses.Failed && message.CreatedAt >= lastHour);
            var pendingResults = await _context.ElectionResults.CountAsync(result => result.Status == ElectionResultStatuses.PendingValidation);
            var complianceBacklog = await _context.ComplianceReminders.CountAsync(reminder => reminder.Status == CampaignMessageStatuses.Queued);
            var activeIncidents = await _context.CampaignMessages.CountAsync(message => message.Title != null && message.Title.Contains("Escalation", StringComparison.OrdinalIgnoreCase) && message.CreatedAt >= now.AddHours(-24));
            var geoPingsLastHour = await _context.CampaignGeoPings.CountAsync(ping => ping.CapturedAt >= lastHour);

            var healthScore = 100;
            healthScore -= Math.Min(35, failedMessages * 3);
            healthScore -= Math.Min(25, pendingResults * 2);
            healthScore -= Math.Min(20, complianceBacklog / 10);
            healthScore -= Math.Min(20, queuedMessages / 20);
            healthScore = Math.Max(0, healthScore);

            return Ok(new
            {
                timestamp = now,
                healthScore,
                queuedMessages,
                failedMessagesLastHour = failedMessages,
                pendingElectionValidation = pendingResults,
                complianceBacklog,
                escalationSignals24h = activeIncidents,
                geoPingsLastHour,
                status = healthScore >= 80 ? "Healthy" : healthScore >= 60 ? "Degraded" : "Critical"
            });
        }
    }
}
