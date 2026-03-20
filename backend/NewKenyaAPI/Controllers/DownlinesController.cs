using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using NewKenyaAPI.Services;
using System.Security.Claims;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DownlinesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CampaignHierarchyService _hierarchyService;
        private readonly AuditLogService _auditLogService;

        public DownlinesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            CampaignHierarchyService hierarchyService,
            AuditLogService auditLogService)
        {
            _context = context;
            _userManager = userManager;
            _hierarchyService = hierarchyService;
            _auditLogService = auditLogService;
        }

        [HttpPost("add")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> AddDownline([FromBody] AddDownlineRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            var canManageLeader = await CanManageHierarchyNodeAsync(currentUserId, request.LeaderUserId);
            if (!canManageLeader)
            {
                return Forbid();
            }

            var validation = await _hierarchyService.ValidateDownlineAssignmentAsync(request.LeaderUserId, request.DownlineUserId);
            if (!validation.IsValid)
            {
                return BadRequest(new { message = validation.Error });
            }

            var leader = await _context.Users.FirstAsync(user => user.Id == request.LeaderUserId);
            var downline = await _context.Users.FirstAsync(user => user.Id == request.DownlineUserId);

            downline.ParentUserId = leader.Id;
            leader.DownlineCount += 1;

            await _context.SaveChangesAsync();

            await _auditLogService.WriteAsync(
                currentUserId,
                "DownlineAdded",
                "Hierarchy",
                downline.Id,
                new { leaderUserId = leader.Id, downlineUserId = downline.Id },
                "DownlinesController.AddDownline");

            return Ok(new
            {
                message = "Downline linked successfully.",
                leader = new
                {
                    leader.Id,
                    fullName = BuildFullName(leader),
                    leader.CampaignRole,
                    leader.DownlineCount
                },
                downline = new
                {
                    downline.Id,
                    fullName = BuildFullName(downline),
                    downline.CampaignRole,
                    downline.ParentUserId
                }
            });
        }

        [HttpPost("reassign")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> ReassignDownline([FromBody] ReassignDownlineRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            var canManageCurrentLeader = await CanManageHierarchyNodeAsync(currentUserId, request.CurrentLeaderUserId);
            var canManageNewLeader = await CanManageHierarchyNodeAsync(currentUserId, request.NewLeaderUserId);
            if (!canManageCurrentLeader || !canManageNewLeader)
            {
                return Forbid();
            }

            var downline = await _context.Users.FirstOrDefaultAsync(user => user.Id == request.DownlineUserId);
            if (downline == null)
            {
                return NotFound(new { message = "Downline user not found." });
            }

            if (downline.ParentUserId != request.CurrentLeaderUserId)
            {
                return BadRequest(new { message = "Downline is not currently assigned to the specified current leader." });
            }

            var validation = await _hierarchyService.ValidateDownlineAssignmentAsync(request.NewLeaderUserId, request.DownlineUserId);
            if (!validation.IsValid)
            {
                return BadRequest(new { message = validation.Error });
            }

            var currentLeader = await _context.Users.FirstAsync(user => user.Id == request.CurrentLeaderUserId);
            var newLeader = await _context.Users.FirstAsync(user => user.Id == request.NewLeaderUserId);

            currentLeader.DownlineCount = Math.Max(0, currentLeader.DownlineCount - 1);
            newLeader.DownlineCount += 1;
            downline.ParentUserId = newLeader.Id;

            await _context.SaveChangesAsync();

            await _auditLogService.WriteAsync(
                currentUserId,
                "DownlineReassigned",
                "Hierarchy",
                downline.Id,
                new { fromLeader = currentLeader.Id, toLeader = newLeader.Id, downlineUserId = downline.Id },
                "DownlinesController.ReassignDownline");

            return Ok(new
            {
                message = "Downline reassigned successfully.",
                downlineUserId = downline.Id,
                fromLeaderUserId = currentLeader.Id,
                toLeaderUserId = newLeader.Id
            });
        }

        [HttpPost("remove")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> RemoveDownline([FromBody] RemoveDownlineRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            var canManageLeader = await CanManageHierarchyNodeAsync(currentUserId, request.LeaderUserId);
            if (!canManageLeader)
            {
                return Forbid();
            }

            var leader = await _context.Users.FirstOrDefaultAsync(user => user.Id == request.LeaderUserId);
            var downline = await _context.Users.FirstOrDefaultAsync(user => user.Id == request.DownlineUserId);
            if (leader == null || downline == null)
            {
                return NotFound(new { message = "Leader or downline not found." });
            }

            if (downline.ParentUserId != leader.Id)
            {
                return BadRequest(new { message = "Downline is not directly assigned to the provided leader." });
            }

            downline.ParentUserId = null;
            leader.DownlineCount = Math.Max(0, leader.DownlineCount - 1);
            await _context.SaveChangesAsync();

            await _auditLogService.WriteAsync(
                currentUserId,
                "DownlineRemoved",
                "Hierarchy",
                downline.Id,
                new { leaderUserId = leader.Id, downlineUserId = downline.Id },
                "DownlinesController.RemoveDownline");

            return Ok(new
            {
                message = "Downline removed from leader successfully.",
                leaderUserId = leader.Id,
                downlineUserId = downline.Id
            });
        }

        [HttpGet("leader-capacity")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> GetLeaderCapacitySummary()
        {
            var leaders = await _context.Users
                .Where(user => user.CampaignRole != UserRoles.User && user.CampaignRole != UserRoles.Volunteer)
                .OrderBy(user => user.FirstName)
                .ThenBy(user => user.LastName)
                .Take(300)
                .ToListAsync();

            var items = leaders.Select(leader => new
            {
                leader.Id,
                fullName = BuildFullName(leader),
                leader.CampaignRole,
                leader.VerificationStatus,
                directDownlines = leader.DownlineCount,
                maxDownlines = 10,
                remainingCapacity = Math.Max(0, 10 - leader.DownlineCount)
            });

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetDownlineNode(string id)
        {
            var user = await _context.Users
                .Where(candidate => candidate.Id == id)
                .Select(candidate => new
                {
                    candidate.Id,
                    fullName = BuildFullName(candidate),
                    candidate.Email,
                    candidate.PhoneNumber,
                    candidate.CampaignRole,
                    candidate.VerificationStatus,
                    candidate.VoterCardStatus,
                    candidate.Region,
                    candidate.County,
                    candidate.SubCounty,
                    candidate.Constituency,
                    candidate.Ward,
                    candidate.PollingStation,
                    candidate.ParentUserId,
                    candidate.DownlineCount,
                    childCount = _context.Users.Count(child => child.ParentUserId == candidate.Id)
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("tree")]
        public async Task<ActionResult<object>> GetTree([FromQuery] string? rootUserId, [FromQuery] int maxDepth = 3)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            var rootId = string.IsNullOrWhiteSpace(rootUserId) ? currentUserId : rootUserId;
            var canManageRoot = await CanManageHierarchyNodeAsync(currentUserId, rootId);
            if (!canManageRoot)
            {
                return Forbid();
            }

            var root = await BuildTreeAsync(rootId, Math.Clamp(maxDepth, 1, 6));
            if (root == null)
            {
                return NotFound();
            }

            return Ok(root);
        }

        private async Task<object?> BuildTreeAsync(string rootUserId, int maxDepth)
        {
            var users = await _context.Users
                .Where(user => user.Id == rootUserId || user.ParentUserId == rootUserId)
                .OrderBy(user => user.FirstName)
                .ThenBy(user => user.LastName)
                .ToListAsync();

            var root = users.FirstOrDefault(user => user.Id == rootUserId) ?? await _context.Users.FirstOrDefaultAsync(user => user.Id == rootUserId);
            if (root == null)
            {
                return null;
            }

            if (maxDepth == 1)
            {
                return ToTreeNode(root, new List<object>());
            }

            var directChildren = users.Where(user => user.ParentUserId == rootUserId).ToList();
            var children = new List<object>();

            foreach (var child in directChildren)
            {
                var childNode = await BuildTreeAsync(child.Id, maxDepth - 1);
                if (childNode != null)
                {
                    children.Add(childNode);
                }
            }

            return ToTreeNode(root, children);
        }

        private object ToTreeNode(ApplicationUser user, List<object> children)
        {
            return new
            {
                user.Id,
                fullName = BuildFullName(user),
                user.CampaignRole,
                user.VerificationStatus,
                user.VoterCardStatus,
                user.Region,
                user.County,
                user.SubCounty,
                user.Constituency,
                user.Ward,
                user.PollingStation,
                user.DownlineCount,
                children
            };
        }

        private async Task<bool> CanManageHierarchyNodeAsync(string currentUserId, string targetUserId)
        {
            if (currentUserId == targetUserId)
            {
                return true;
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null)
            {
                return false;
            }

            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            if (currentRoles.Contains(UserRoles.Admin) || currentRoles.Contains(UserRoles.SuperAdmin))
            {
                return true;
            }

            return await _hierarchyService.IsDownlineAsync(currentUserId, targetUserId);
        }

        private static string BuildFullName(ApplicationUser user)
        {
            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            return string.IsNullOrWhiteSpace(fullName) ? user.Email ?? user.UserName ?? "Unknown User" : fullName;
        }
    }
}