using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Services
{
    public class CampaignMessagingService
    {
        private readonly ApplicationDbContext _context;

        public CampaignMessagingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> QueueTargetMessagesAsync(string senderUserId, MessageTargetRequest request)
        {
            if (request.ScheduledFor.HasValue && request.ScheduledFor.Value < DateTime.UtcNow.AddMinutes(-1))
            {
                throw new InvalidOperationException("Scheduled time must be in the future.");
            }

            var query = _context.Users.AsQueryable();

            if (request.ReceiverUserIds != null && request.ReceiverUserIds.Count > 0)
            {
                query = query.Where(user => request.ReceiverUserIds.Contains(user.Id));
            }

            if (!string.IsNullOrWhiteSpace(request.Region))
            {
                query = query.Where(user => user.Region == request.Region);
            }

            if (!string.IsNullOrWhiteSpace(request.County))
            {
                query = query.Where(user => user.County == request.County);
            }

            if (!string.IsNullOrWhiteSpace(request.CampaignRole))
            {
                query = query.Where(user => user.CampaignRole == request.CampaignRole);
            }

            var recipients = await query.Select(user => user.Id).ToListAsync();
            if (recipients.Count == 0)
            {
                return 0;
            }

            var now = DateTime.UtcNow;
            var dispatchTime = request.ScheduledFor?.ToUniversalTime() ?? now;
            var messages = recipients.Select(receiverId => new CampaignMessage
            {
                SenderUserId = senderUserId,
                ReceiverUserId = receiverId,
                Channel = request.Channel,
                Title = request.Title,
                Body = request.Body,
                Url = request.Url,
                Status = CampaignMessageStatuses.Queued,
                NextAttemptAt = dispatchTime,
                CreatedAt = now
            }).ToList();

            _context.CampaignMessages.AddRange(messages);
            await _context.SaveChangesAsync();
            return messages.Count;
        }

        public async Task<int> QueueBroadcastAsync(string senderUserId, MessageBroadcastRequest request)
        {
            var target = new MessageTargetRequest
            {
                Channel = request.Channel,
                Title = request.Title,
                Body = request.Body,
                Url = request.Url,
                ScheduledFor = request.ScheduledFor
            };

            return await QueueTargetMessagesAsync(senderUserId, target);
        }
    }
}
