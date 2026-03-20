using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Services
{
    public class LeaderboardService
    {
        private readonly ApplicationDbContext _context;

        public LeaderboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> RecalculateAsync()
        {
            var users = await _context.Users.ToListAsync();
            var completedTaskCounts = await _context.Tasks
                .Where(task => task.Status == CampaignTaskStatuses.Completed && task.AssignedToUserId != null)
                .GroupBy(task => task.AssignedToUserId!)
                .ToDictionaryAsync(group => group.Key, group => group.Count());

            var verifiedVoterCounts = users
                .Where(user => user.VoterCardStatus == CampaignVoterCardStatuses.Verified)
                .GroupBy(user => user.Id)
                .ToDictionary(group => group.Key, group => group.Count());

            var existingScores = await _context.LeaderboardScores.ToListAsync();

            foreach (var user in users)
            {
                var tasksCompleted = completedTaskCounts.TryGetValue(user.Id, out var taskCount) ? taskCount : 0;
                var verifiedVoters = verifiedVoterCounts.TryGetValue(user.Id, out var voterCount) ? voterCount : 0;

                var verificationIntegrityPoints = 0;
                if (string.Equals(user.VerificationStatus, CampaignVerificationStatuses.Verified, StringComparison.OrdinalIgnoreCase))
                {
                    verificationIntegrityPoints += 40;
                }

                if (string.Equals(user.VoterCardStatus, CampaignVoterCardStatuses.Verified, StringComparison.OrdinalIgnoreCase))
                {
                    verificationIntegrityPoints += 30;
                }

                if (user.DownlineCount > 0)
                {
                    verificationIntegrityPoints += user.DownlineCount * 2;
                }

                var totalPoints = (user.DownlineCount * 10) + (tasksCompleted * 5) + (verifiedVoters * 20) + verificationIntegrityPoints;

                var score = existingScores.FirstOrDefault(item => item.UserId == user.Id);
                if (score == null)
                {
                    score = new LeaderboardScore { UserId = user.Id };
                    _context.LeaderboardScores.Add(score);
                }

                score.TotalPoints = totalPoints;
                score.DirectDownlines = user.DownlineCount;
                score.TasksCompleted = tasksCompleted;
                score.VerifiedVoterCards = verifiedVoters;
                score.VerificationIntegrityPoints = verificationIntegrityPoints;
                score.BadgeTier = ResolveBadgeTier(totalPoints);
                score.RecognitionTitle = ResolveRecognitionTitle(totalPoints, tasksCompleted, user.DownlineCount);
                score.IncentiveTag = ResolveIncentiveTag(totalPoints);
                score.Scope = "National";
                score.Region = user.Region;
                score.County = user.County;
                score.LastCalculatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return users.Count;
        }

        private static string ResolveBadgeTier(int points)
        {
            return points switch
            {
                >= 700 => "Diamond",
                >= 500 => "Platinum",
                >= 320 => "Gold",
                >= 180 => "Silver",
                _ => "Bronze"
            };
        }

        private static string ResolveRecognitionTitle(int points, int tasksCompleted, int downlines)
        {
            if (points >= 700)
            {
                return "National Command Champion";
            }

            if (tasksCompleted >= 20)
            {
                return "Operations Finisher";
            }

            if (downlines >= 10)
            {
                return "Network Builder";
            }

            return "Field Contributor";
        }

        private static string ResolveIncentiveTag(int points)
        {
            return points switch
            {
                >= 700 => "Priority Strategy Briefing Invite",
                >= 500 => "Regional Recognition + Digital Feature",
                >= 320 => "County Recognition Certificate",
                >= 180 => "Leadership Mention",
                _ => "Weekly Recognition"
            };
        }
    }
}
