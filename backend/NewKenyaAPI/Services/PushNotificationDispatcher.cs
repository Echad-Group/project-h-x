using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using WebPush;
using System.Text.Json;

namespace NewKenyaAPI.Services
{
    public class PushNotificationDispatcher
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PushNotificationDispatcher> _logger;

        public PushNotificationDispatcher(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<PushNotificationDispatcher> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task DispatchCategoryAsync(string category, string title, string body, string? url, CancellationToken cancellationToken = default)
        {
            var publicKey = _configuration["VapidKeys:PublicKey"];
            var privateKey = _configuration["VapidKeys:PrivateKey"];
            var subject = _configuration["VapidKeys:Subject"];

            if (string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(privateKey) || string.IsNullOrWhiteSpace(subject))
            {
                _logger.LogWarning("Skipping push dispatch for category {Category}: VAPID keys not configured", category);
                return;
            }

            var subscriptions = await _context.PushSubscriptions
                .Where(subscription => subscription.IsActive)
                .ToListAsync(cancellationToken);

            if (subscriptions.Count == 0)
            {
                return;
            }

            var webPushClient = new WebPushClient();
            var vapidDetails = new VapidDetails(subject, publicKey, privateKey);

            int successCount = 0;
            int failureCount = 0;

            var payload = JsonSerializer.Serialize(new
            {
                title,
                body,
                url = string.IsNullOrWhiteSpace(url) ? "/" : url,
                icon = "/assets/icons/icon-192.svg",
                badge = "/assets/icons/icon-96.svg",
                category,
                data = new
                {
                    category
                }
            });

            foreach (var subscription in subscriptions)
            {
                try
                {
                    var pushSubscription = new WebPush.PushSubscription(
                        subscription.Endpoint,
                        subscription.P256dh,
                        subscription.Auth
                    );

                    await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails, cancellationToken: cancellationToken);
                    subscription.LastUsed = DateTime.UtcNow;
                    successCount++;
                }
                catch (WebPushException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Gone || ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    subscription.IsActive = false;
                    failureCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Push send failed for category {Category} endpoint {Endpoint}", category, subscription.Endpoint);
                    failureCount++;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Category push dispatched: {Category}, Success={SuccessCount}, Failure={FailureCount}",
                category,
                successCount,
                failureCount
            );
        }
    }
}
