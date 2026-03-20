using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using NewKenyaAPI.Services;
using System.Security.Claims;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly CampaignMessagingService _campaignMessagingService;
        private readonly ApplicationDbContext _context;
        private readonly AuditLogService _auditLogService;

        public MessagesController(CampaignMessagingService campaignMessagingService, ApplicationDbContext context, AuditLogService auditLogService)
        {
            _campaignMessagingService = campaignMessagingService;
            _context = context;
            _auditLogService = auditLogService;
        }

        [HttpPost("broadcast")]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        public async Task<ActionResult<object>> Broadcast([FromBody] MessageBroadcastRequest request)
        {
            var senderUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(senderUserId))
            {
                return Unauthorized();
            }

            try
            {
                var queuedCount = await _campaignMessagingService.QueueBroadcastAsync(senderUserId, request);
                await _auditLogService.WriteAsync(senderUserId, "MessageBroadcastQueued", "CampaignMessage", null, new { request.Channel, queuedCount, request.ScheduledFor }, "MessagesController.Broadcast");
                return Ok(new
                {
                    message = request.ScheduledFor.HasValue ? "Broadcast scheduled." : "Broadcast queued.",
                    queuedCount,
                    scheduledFor = request.ScheduledFor
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("target")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> Target([FromBody] MessageTargetRequest request)
        {
            var senderUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(senderUserId))
            {
                return Unauthorized();
            }

            try
            {
                var queuedCount = await _campaignMessagingService.QueueTargetMessagesAsync(senderUserId, request);
                await _auditLogService.WriteAsync(senderUserId, "MessageTargetQueued", "CampaignMessage", null, new { request.Channel, queuedCount, request.ScheduledFor }, "MessagesController.Target");
                return Ok(new
                {
                    message = request.ScheduledFor.HasValue ? "Target message scheduled." : "Target message queued.",
                    queuedCount,
                    scheduledFor = request.ScheduledFor
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("analytics")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> GetAnalytics()
        {
            var since = DateTime.UtcNow.AddDays(-14);
            var recent = _context.CampaignMessages.Where(message => message.CreatedAt >= since);

            var totalsByStatus = await recent
                .GroupBy(message => message.Status)
                .Select(group => new { status = group.Key, count = group.Count() })
                .ToListAsync();

            var totalsByChannel = await recent
                .GroupBy(message => message.Channel)
                .Select(group => new
                {
                    channel = group.Key,
                    queued = group.Count(message => message.Status == CampaignMessageStatuses.Queued),
                    sent = group.Count(message => message.SentAt != null),
                    delivered = group.Count(message => message.DeliveredAt != null),
                    read = group.Count(message => message.ReadAt != null),
                    failed = group.Count(message => message.Status == CampaignMessageStatuses.Failed)
                })
                .ToListAsync();

            var recentFailures = await recent
                .Where(message => message.Status == CampaignMessageStatuses.Failed)
                .OrderByDescending(message => message.CreatedAt)
                .Take(20)
                .Select(message => new
                {
                    message.Id,
                    message.Channel,
                    message.CreatedAt,
                    message.LastError,
                    message.DeadLetterReason,
                    message.RetryCount
                })
                .ToListAsync();

            return Ok(new
            {
                windowDays = 14,
                totalsByStatus,
                totalsByChannel,
                recentFailures
            });
        }

        [HttpGet("inbox")]
        public async Task<ActionResult<object>> GetInbox()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var messages = await _context.CampaignMessages
                .Where(message => message.ReceiverUserId == userId)
                .OrderByDescending(message => message.CreatedAt)
                .Take(200)
                .Select(message => new
                {
                    message.Id,
                    message.Channel,
                    message.Title,
                    message.Body,
                    message.Url,
                    message.Status,
                    message.CreatedAt,
                    message.DeliveredAt,
                    message.ReadAt
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost("{messageId:int}/read")]
        public async Task<ActionResult<object>> AcknowledgeRead(int messageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var message = await _context.CampaignMessages.FirstOrDefaultAsync(item => item.Id == messageId);
            if (message == null)
            {
                return NotFound(new { message = "Message not found." });
            }

            if (message.ReceiverUserId != userId)
            {
                return Forbid();
            }

            message.ReadAt = DateTime.UtcNow;
            if (message.Status != CampaignMessageStatuses.Failed)
            {
                message.Status = CampaignMessageStatuses.Read;
            }

            await _context.SaveChangesAsync();

            await _auditLogService.WriteAsync(userId, "MessageReadAcknowledged", "CampaignMessage", message.Id.ToString(), null, "MessagesController.AcknowledgeRead");

            return Ok(new { message = "Read acknowledged.", messageId = message.Id, readAt = message.ReadAt });
        }
    }
}
