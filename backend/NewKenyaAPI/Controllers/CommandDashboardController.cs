using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Controllers
{
    [Route("api/command-dashboard")]
    [ApiController]
    [Authorize(Roles = UserRoles.LeadershipAccess)]
    public class CommandDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommandDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetSummary()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalLeaders = await _context.Users.CountAsync(user => user.DownlineCount > 0);
            var totalVolunteers = await _context.Users.CountAsync(user => user.CampaignRole == UserRoles.Volunteer);
            var verifiedUsers = await _context.Users.CountAsync(user => user.VerificationStatus == CampaignVerificationStatuses.Verified);
            var pendingVerification = await _context.Users.CountAsync(user => user.VerificationStatus == CampaignVerificationStatuses.Pending);
            var compliantUsers = await _context.Users.CountAsync(user => user.VoterCardStatus == CampaignVoterCardStatuses.Verified);
            var missingVoterCards = await _context.Users.CountAsync(user => user.VoterCardStatus == CampaignVoterCardStatuses.Missing);
            var totalTasks = await _context.Tasks.CountAsync();
            var activeTasks = await _context.Tasks.CountAsync(task => task.Status == CampaignTaskStatuses.Pending || task.Status == CampaignTaskStatuses.InProgress);
            var overdueTasks = await _context.Tasks.CountAsync(task => task.DueDate.HasValue && task.DueDate < DateTime.UtcNow && task.Status != CampaignTaskStatuses.Completed);
            var reminderBacklog = await _context.ComplianceReminders.CountAsync(reminder => reminder.Status == CampaignMessageStatuses.Queued);

            var topLeaders = await _context.Users
                .Where(user => user.DownlineCount > 0)
                .OrderByDescending(user => user.DownlineCount)
                .ThenBy(user => user.FirstName)
                .Take(8)
                .Select(user => new
                {
                    user.Id,
                    fullName = BuildFullName(user),
                    user.CampaignRole,
                    user.Region,
                    user.County,
                    user.DownlineCount,
                    user.VerificationStatus,
                    user.VoterCardStatus
                })
                .ToListAsync();

            var verificationDistribution = await _context.Users
                .GroupBy(user => user.VerificationStatus)
                .Select(group => new { status = group.Key, count = group.Count() })
                .ToListAsync();

            var taskDistribution = await _context.Tasks
                .GroupBy(task => task.Status)
                .Select(group => new { status = group.Key, count = group.Count() })
                .ToListAsync();

            var regionBreakdown = await _context.Users
                .GroupBy(user => user.Region)
                .Select(group => new
                {
                    region = group.Key ?? "Unspecified",
                    users = group.Count(),
                    verified = group.Count(user => user.VerificationStatus == CampaignVerificationStatuses.Verified),
                    compliant = group.Count(user => user.VoterCardStatus == CampaignVoterCardStatuses.Verified)
                })
                .OrderByDescending(item => item.users)
                .ToListAsync();

            var countyBreakdown = await _context.Users
                .GroupBy(user => new { user.Region, user.County })
                .Select(group => new
                {
                    region = group.Key.Region ?? "Unspecified",
                    county = group.Key.County ?? "Unspecified",
                    users = group.Count(),
                    verified = group.Count(user => user.VerificationStatus == CampaignVerificationStatuses.Verified)
                })
                .OrderByDescending(item => item.users)
                .Take(120)
                .ToListAsync();

            var subCountyBreakdown = await _context.Users
                .Where(user => user.SubCounty != null)
                .GroupBy(user => new { user.County, user.SubCounty })
                .Select(group => new
                {
                    county = group.Key.County ?? "Unspecified",
                    subCounty = group.Key.SubCounty ?? "Unspecified",
                    users = group.Count(),
                    verified = group.Count(user => user.VerificationStatus == CampaignVerificationStatuses.Verified)
                })
                .OrderByDescending(item => item.users)
                .Take(200)
                .ToListAsync();

            var wardBreakdown = await _context.Users
                .Where(user => user.Ward != null)
                .GroupBy(user => new { user.SubCounty, user.Ward })
                .Select(group => new
                {
                    subCounty = group.Key.SubCounty ?? "Unspecified",
                    ward = group.Key.Ward ?? "Unspecified",
                    users = group.Count(),
                    missingVoterCards = group.Count(user => user.VoterCardStatus == CampaignVoterCardStatuses.Missing)
                })
                .OrderByDescending(item => item.users)
                .Take(300)
                .ToListAsync();

            return Ok(new
            {
                totalUsers,
                totalLeaders,
                totalVolunteers,
                verifiedUsers,
                pendingVerification,
                compliantUsers,
                missingVoterCards,
                totalTasks,
                activeTasks,
                overdueTasks,
                reminderBacklog,
                topLeaders,
                verificationDistribution,
                taskDistribution,
                regionBreakdown,
                countyBreakdown,
                subCountyBreakdown,
                wardBreakdown
            });
        }

        private static string BuildFullName(ApplicationUser user)
        {
            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            return string.IsNullOrWhiteSpace(fullName) ? user.Email ?? user.UserName ?? "Unknown User" : fullName;
        }
    }
}