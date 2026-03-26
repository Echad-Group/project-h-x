using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebPush;
using Task = System.Threading.Tasks.Task;

namespace NewKenyaAPI.Services
{
    public class MessageDeliveryWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MessageDeliveryWorker> _logger;

        public MessageDeliveryWorker(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<MessageDeliveryWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessBatchAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Message delivery batch failed");
                }
                
                // TODO: Implement a more sophisticated scheduling mechanism (e.g. exponential backoff, dynamic delay based on load)
                await Task.Delay(TimeSpan.FromSeconds(50), stoppingToken);
            }
        }

        private async Task ProcessBatchAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var now = DateTime.UtcNow;
            var messages = await context.CampaignMessages
                .Where(m => m.Status == CampaignMessageStatuses.Queued && (m.NextAttemptAt == null || m.NextAttemptAt <= now))
                .OrderBy(m => m.CreatedAt)
                .Take(100)
                .ToListAsync(cancellationToken);

            if (messages.Count == 0)
            {
                return;
            }

            foreach (var message in messages)
            {
                message.Status = CampaignMessageStatuses.Processing;
            }

            await context.SaveChangesAsync(cancellationToken);

            foreach (var message in messages)
            {
                try
                {
                    await DeliverMessageAsync(context, message, cancellationToken);
                }
                catch (Exception ex)
                {
                    message.RetryCount += 1;
                    message.LastError = ex.Message;
                    message.Status = message.RetryCount >= message.MaxRetries
                        ? CampaignMessageStatuses.Failed
                        : CampaignMessageStatuses.Queued;

                    if (message.Status == CampaignMessageStatuses.Failed)
                    {
                        message.DeadLetterReason = ex.Message;
                    }
                    else
                    {
                        message.NextAttemptAt = DateTime.UtcNow.AddSeconds(30 * message.RetryCount);
                    }
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        private async Task DeliverMessageAsync(ApplicationDbContext context, CampaignMessage message, CancellationToken cancellationToken)
        {
            var receiver = await context.Users.FirstOrDefaultAsync(u => u.Id == message.ReceiverUserId, cancellationToken);
            if (receiver == null)
            {
                message.Status = CampaignMessageStatuses.Failed;
                message.DeadLetterReason = "Receiver not found";
                return;
            }

            if (message.Channel == CampaignMessageChannels.InApp)
            {
                message.Status = CampaignMessageStatuses.Delivered;
                message.SentAt = DateTime.UtcNow;
                message.DeliveredAt = DateTime.UtcNow;
                return;
            }

            if (message.Channel == CampaignMessageChannels.Push)
            {
                await SendPushAsync(context, receiver, message, cancellationToken);
                return;
            }

            if (message.Channel == CampaignMessageChannels.WhatsApp)
            {
                await SendWhatsAppAsync(receiver, message, cancellationToken);
                return;
            }

            throw new InvalidOperationException($"Unsupported channel {message.Channel}");
        }

        private async Task SendPushAsync(ApplicationDbContext context, ApplicationUser receiver, CampaignMessage message, CancellationToken cancellationToken)
        {
            var subscriptions = await context.PushSubscriptions
                .Where(p => p.UserId == receiver.Id && p.IsActive)
                .ToListAsync(cancellationToken);

            if (subscriptions.Count == 0)
            {
                throw new InvalidOperationException("No active push subscriptions");
            }

            var publicKey = _configuration["VapidKeys:PublicKey"];
            var privateKey = _configuration["VapidKeys:PrivateKey"];
            var subject = _configuration["VapidKeys:Subject"];
            if (string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(privateKey) || string.IsNullOrWhiteSpace(subject))
            {
                throw new InvalidOperationException("VAPID keys are not configured");
            }

            var webPushClient = new WebPushClient();
            var vapid = new VapidDetails(subject, publicKey, privateKey);

            var payload = JsonSerializer.Serialize(new
            {
                title = message.Title ?? "Campaign Update",
                body = message.Body,
                url = message.Url ?? "/"
            });

            foreach (var sub in subscriptions)
            {
                try
                {
                    var pushSub = new WebPush.PushSubscription(sub.Endpoint, sub.P256dh, sub.Auth);
                    await webPushClient.SendNotificationAsync(pushSub, payload, vapid);
                    sub.LastUsed = DateTime.UtcNow;
                }
                catch (WebPushException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Gone || ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    sub.IsActive = false;
                }
            }

            message.Status = CampaignMessageStatuses.Delivered;
            message.SentAt = DateTime.UtcNow;
            message.DeliveredAt = DateTime.UtcNow;
        }

        private async Task SendWhatsAppAsync(ApplicationUser receiver, CampaignMessage message, CancellationToken cancellationToken)
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromNumber = _configuration["Twilio:WhatsAppFrom"];

            if (string.IsNullOrWhiteSpace(accountSid) || string.IsNullOrWhiteSpace(authToken) || string.IsNullOrWhiteSpace(fromNumber))
            {
                throw new InvalidOperationException("Twilio WhatsApp configuration is missing");
            }

            if (string.IsNullOrWhiteSpace(receiver.PhoneNumber))
            {
                throw new InvalidOperationException("Receiver has no phone number");
            }

            var client = _httpClientFactory.CreateClient("twilio-whatsapp");
            var authBytes = Encoding.UTF8.GetBytes($"{accountSid}:{authToken}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

            var requestBody = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["To"] = NormalizeWhatsAppNumber(receiver.PhoneNumber),
                ["From"] = NormalizeWhatsAppNumber(fromNumber),
                ["Body"] = string.IsNullOrWhiteSpace(message.Title) ? message.Body : $"{message.Title}\n{message.Body}"
            });

            var endpoint = $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Messages.json";
            var response = await client.PostAsync(endpoint, requestBody, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Twilio send failed: {response.StatusCode} {payload}");
            }

            message.Status = CampaignMessageStatuses.Delivered;
            message.SentAt = DateTime.UtcNow;
            message.DeliveredAt = DateTime.UtcNow;
        }

        private static string NormalizeWhatsAppNumber(string number)
        {
            var normalized = number.Trim();
            return normalized.StartsWith("whatsapp:", StringComparison.OrdinalIgnoreCase)
                ? normalized
                : $"whatsapp:{normalized}";
        }
    }
}
