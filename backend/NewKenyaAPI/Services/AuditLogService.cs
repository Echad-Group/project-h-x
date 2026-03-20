using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using System.Text.Json;

namespace NewKenyaAPI.Services
{
    public class AuditLogService
    {
        private readonly ApplicationDbContext _context;

        public AuditLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task WriteAsync(string? actorUserId, string eventType, string? resourceType, string? resourceId, object? details, string source)
        {
            var entry = new AuditLogEvent
            {
                ActorUserId = actorUserId,
                EventType = eventType,
                ResourceType = resourceType,
                ResourceId = resourceId,
                DetailsJson = details == null ? null : JsonSerializer.Serialize(details),
                Source = source,
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogEvents.Add(entry);
            await _context.SaveChangesAsync();
        }
    }
}
