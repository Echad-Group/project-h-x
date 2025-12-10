using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using System.Security.Claims;
using WebPush;
using System.Text.Json;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PushController> _logger;

        public PushController(ApplicationDbContext context, IConfiguration configuration, ILogger<PushController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        // POST: api/Push/subscribe
        [HttpPost("subscribe")]
        public async Task<ActionResult> Subscribe([FromBody] PushSubscriptionRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Check if subscription already exists
                var existing = await _context.PushSubscriptions
                    .FirstOrDefaultAsync(p => p.Endpoint == request.Endpoint);

                if (existing != null)
                {
                    // Update existing subscription
                    existing.P256dh = request.Keys?.P256dh;
                    existing.Auth = request.Keys?.Auth;
                    existing.UserId = userId;
                    existing.IsActive = true;
                    existing.LastUsed = DateTime.UtcNow;
                }
                else
                {
                    // Create new subscription
                    var subscription = new Models.PushSubscription
                    {
                        Endpoint = request.Endpoint,
                        P256dh = request.Keys?.P256dh,
                        Auth = request.Keys?.Auth,
                        UserId = userId,
                        IsActive = true
                    };

                    _context.PushSubscriptions.Add(subscription);
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Subscription saved successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to save subscription", error = ex.Message });
            }
        }

        // POST: api/Push/unsubscribe
        [HttpPost("unsubscribe")]
        public async Task<ActionResult> Unsubscribe([FromBody] UnsubscribeRequest request)
        {
            try
            {
                var subscription = await _context.PushSubscriptions
                    .FirstOrDefaultAsync(p => p.Endpoint == request.Endpoint);

                if (subscription != null)
                {
                    subscription.IsActive = false;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Unsubscribed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to unsubscribe", error = ex.Message });
            }
        }

        // POST: api/Push/send - Admin only
        [HttpPost("send")]
        [Authorize] // Add role-based authorization if needed
        public async Task<ActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            try
            {
                // Get active subscriptions
                var subscriptions = await _context.PushSubscriptions
                    .Where(p => p.IsActive)
                    .ToListAsync();

                if (subscriptions.Count == 0)
                {
                    return Ok(new { message = "No active subscriptions found", recipientCount = 0 });
                }

                // Get VAPID keys from configuration
                var publicKey = _configuration["VapidKeys:PublicKey"];
                var privateKey = _configuration["VapidKeys:PrivateKey"];
                var subject = _configuration["VapidKeys:Subject"];

                if (string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(privateKey))
                {
                    return StatusCode(500, new { message = "VAPID keys not configured" });
                }

                // Create WebPush client
                var webPushClient = new WebPushClient();
                var vapidDetails = new VapidDetails(subject, publicKey, privateKey);

                int successCount = 0;
                int failureCount = 0;
                var failedEndpoints = new List<string>();

                // Send notification to each subscription
                foreach (var subscription in subscriptions)
                {
                    try
                    {
                        var pushSubscription = new WebPush.PushSubscription(
                            subscription.Endpoint,
                            subscription.P256dh,
                            subscription.Auth
                        );

                        // Create notification payload
                        var payload = JsonSerializer.Serialize(new
                        {
                            title = request.Title,
                            body = request.Body,
                            icon = request.Icon ?? "/assets/icons/icon-192.svg",
                            badge = "/assets/icons/icon-96.svg",
                            url = request.Url ?? "/",
                            timestamp = DateTime.UtcNow
                        });

                        // Send the notification
                        await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                        
                        // Update last used timestamp
                        subscription.LastUsed = DateTime.UtcNow;
                        successCount++;

                        _logger.LogInformation($"Push notification sent successfully to endpoint: {subscription.Endpoint.Substring(0, 50)}...");
                    }
                    catch (WebPushException ex)
                    {
                        _logger.LogError($"Failed to send push notification: {ex.Message}");
                        failureCount++;
                        failedEndpoints.Add(subscription.Endpoint);

                        // If subscription is no longer valid (410 Gone or 404 Not Found), mark as inactive
                        if (ex.StatusCode == System.Net.HttpStatusCode.Gone || 
                            ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            subscription.IsActive = false;
                            _logger.LogInformation($"Marked subscription as inactive due to {ex.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Unexpected error sending push notification: {ex.Message}");
                        failureCount++;
                        failedEndpoints.Add(subscription.Endpoint);
                    }
                }

                // Save changes (updated timestamps and inactive subscriptions)
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    message = "Notification sent",
                    totalSubscriptions = subscriptions.Count,
                    successCount,
                    failureCount,
                    notification = request
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send notifications: {ex.Message}");
                return BadRequest(new { message = "Failed to send notification", error = ex.Message });
            }
        }

        // GET: api/Push/status
        [HttpGet("status")]
        [Authorize]
        public async Task<ActionResult> GetStatus()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var subscription = await _context.PushSubscriptions
                .Where(p => p.UserId == userId && p.IsActive)
                .FirstOrDefaultAsync();

            return Ok(new
            {
                isSubscribed = subscription != null,
                subscriptionDate = subscription?.CreatedAt
            });
        }
    }

    // Request models
    public class PushSubscriptionRequest
    {
        public string Endpoint { get; set; } = string.Empty;
        public PushKeys? Keys { get; set; }
    }

    public class PushKeys
    {
        public string? P256dh { get; set; }
        public string? Auth { get; set; }
    }

    public class UnsubscribeRequest
    {
        public string Endpoint { get; set; } = string.Empty;
    }

    public class NotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Icon { get; set; }
    }
}
