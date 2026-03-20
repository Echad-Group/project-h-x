using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using Task = System.Threading.Tasks.Task;

namespace NewKenyaAPI.Services
{
    public class WeeklyCommandReportingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WeeklyCommandReportingService> _logger;
        private string? _lastIsoWeek;

        public WeeklyCommandReportingService(IServiceProvider serviceProvider, ILogger<WeeklyCommandReportingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await GenerateWeeklyReportIfDue(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Weekly command report job failed");
                }

                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
        }

        private async Task GenerateWeeklyReportIfDue(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var isoWeek = $"{now:yyyy}-W{System.Globalization.ISOWeek.GetWeekOfYear(now):00}";
            if (_lastIsoWeek == isoWeek)
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var totalUsers = await context.Users.CountAsync(cancellationToken);
            var verifiedUsers = await context.Users.CountAsync(user => user.VerificationStatus == CampaignVerificationStatuses.Verified, cancellationToken);
            var activeTasks = await context.Tasks.CountAsync(task => task.Status == CampaignTaskStatuses.Pending || task.Status == CampaignTaskStatuses.InProgress, cancellationToken);
            var completedTasks = await context.Tasks.CountAsync(task => task.Status == CampaignTaskStatuses.Completed, cancellationToken);
            var complianceBacklog = await context.Users.CountAsync(user => user.VoterCardStatus == CampaignVoterCardStatuses.Missing, cancellationToken);

            var reportBody = $"Weekly Command Report ({isoWeek})\n"
                + $"- Total users: {totalUsers}\n"
                + $"- Verified users: {verifiedUsers}\n"
                + $"- Active tasks: {activeTasks}\n"
                + $"- Completed tasks: {completedTasks}\n"
                + $"- Compliance backlog: {complianceBacklog}";

            var adminIds = await context.Users
                .Where(user => user.CampaignRole == UserRoles.Admin || user.CampaignRole == UserRoles.SuperAdmin)
                .Select(user => user.Id)
                .Take(10)
                .ToListAsync(cancellationToken);

            if (adminIds.Count > 0)
            {
                var senderId = adminIds[0];
                var messages = adminIds.Select(adminId => new CampaignMessage
                {
                    SenderUserId = senderId,
                    ReceiverUserId = adminId,
                    Channel = CampaignMessageChannels.InApp,
                    Title = $"Weekly Command Report {isoWeek}",
                    Body = reportBody,
                    Status = CampaignMessageStatuses.Queued,
                    NextAttemptAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                context.CampaignMessages.AddRange(messages);
                await context.SaveChangesAsync(cancellationToken);
            }

            _lastIsoWeek = isoWeek;
            _logger.LogInformation("Weekly command report generated for {Week}", isoWeek);
        }
    }
}
