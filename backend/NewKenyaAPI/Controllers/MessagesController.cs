using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public MessagesController(CampaignMessagingService campaignMessagingService)
        {
            _campaignMessagingService = campaignMessagingService;
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

            var queuedCount = await _campaignMessagingService.QueueBroadcastAsync(senderUserId, request);
            return Ok(new { message = "Broadcast queued.", queuedCount });
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

            var queuedCount = await _campaignMessagingService.QueueTargetMessagesAsync(senderUserId, request);
            return Ok(new { message = "Target message queued.", queuedCount });
        }
    }
}
