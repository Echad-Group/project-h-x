using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Services
{
    public class CampaignHierarchyService
    {
        private readonly ApplicationDbContext _context;

        public CampaignHierarchyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsValid, string? Error)> ValidateDownlineAssignmentAsync(string leaderUserId, string downlineUserId)
        {
            if (leaderUserId == downlineUserId)
            {
                return (false, "A user cannot be their own leader.");
            }

            var leader = await _context.Users.FirstOrDefaultAsync(user => user.Id == leaderUserId);
            var downline = await _context.Users.FirstOrDefaultAsync(user => user.Id == downlineUserId);

            if (leader == null || downline == null)
            {
                return (false, "Leader or downline user was not found.");
            }

            if (!string.Equals(leader.VerificationStatus, CampaignVerificationStatuses.Verified, StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Only verified leaders can recruit downlines.");
            }

            if (!string.Equals(downline.VerificationStatus, CampaignVerificationStatuses.Verified, StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Only verified members can be recruited as downlines.");
            }

            if (!string.IsNullOrWhiteSpace(downline.ParentUserId))
            {
                return (false, "This user already belongs to another leader.");
            }

            if (leader.DownlineCount >= 10)
            {
                return (false, "The leader already has the maximum 10 direct downlines.");
            }

            if (await IsDescendantAsync(downlineUserId, leaderUserId))
            {
                return (false, "This assignment would create a circular hierarchy.");
            }

            if (!IsWithinGeography(leader, downline))
            {
                return (false, "Leader and downline do not belong to the same geographic hierarchy.");
            }

            return (true, null);
        }

        public async Task<bool> IsDownlineAsync(string leaderUserId, string targetUserId)
        {
            if (leaderUserId == targetUserId)
            {
                return true;
            }

            var descendantIds = await GetDescendantIdsAsync(leaderUserId);
            return descendantIds.Contains(targetUserId);
        }

        public async Task<HashSet<string>> GetDescendantIdsAsync(string rootUserId)
        {
            var descendants = new HashSet<string>();
            var frontier = new Queue<string>();
            frontier.Enqueue(rootUserId);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                var children = await _context.Users
                    .Where(user => user.ParentUserId == current)
                    .Select(user => user.Id)
                    .ToListAsync();

                foreach (var childId in children)
                {
                    if (descendants.Add(childId))
                    {
                        frontier.Enqueue(childId);
                    }
                }
            }

            return descendants;
        }

        public async Task<bool> IsDescendantAsync(string potentialAncestorId, string potentialDescendantId)
        {
            var currentUserId = potentialDescendantId;

            while (!string.IsNullOrWhiteSpace(currentUserId))
            {
                var current = await _context.Users
                    .Where(user => user.Id == currentUserId)
                    .Select(user => new { user.Id, user.ParentUserId })
                    .FirstOrDefaultAsync();

                if (current == null || string.IsNullOrWhiteSpace(current.ParentUserId))
                {
                    return false;
                }

                if (current.ParentUserId == potentialAncestorId)
                {
                    return true;
                }

                currentUserId = current.ParentUserId;
            }

            return false;
        }

        private static bool IsWithinGeography(ApplicationUser leader, ApplicationUser downline)
        {
            return Matches(leader.Region, downline.Region)
                && Matches(leader.County, downline.County)
                && Matches(leader.SubCounty, downline.SubCounty)
                && Matches(leader.Constituency, downline.Constituency)
                && Matches(leader.Ward, downline.Ward)
                && Matches(leader.PollingStation, downline.PollingStation);
        }

        private static bool Matches(string? leaderValue, string? downlineValue)
        {
            if (string.IsNullOrWhiteSpace(leaderValue) || string.IsNullOrWhiteSpace(downlineValue))
            {
                return true;
            }

            return string.Equals(leaderValue.Trim(), downlineValue.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}