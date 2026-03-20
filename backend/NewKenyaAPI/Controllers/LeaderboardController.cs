using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = UserRoles.LeadershipAccess)]
    public class LeaderboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly LeaderboardService _leaderboardService;

        public LeaderboardController(ApplicationDbContext context, LeaderboardService leaderboardService)
        {
            _context = context;
            _leaderboardService = leaderboardService;
        }

        [HttpPost("recalculate")]
        public async Task<ActionResult<object>> Recalculate()
        {
            var processed = await _leaderboardService.RecalculateAsync();
            return Ok(new { message = "Leaderboard recalculated.", processed });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Get([FromQuery] string scope = "National", [FromQuery] string? region = null, [FromQuery] string? county = null)
        {
            var query = _context.LeaderboardScores.Include(score => score.User).AsQueryable();

            query = query.Where(score => score.Scope == scope);

            if (!string.IsNullOrWhiteSpace(region))
            {
                query = query.Where(score => score.Region == region);
            }

            if (!string.IsNullOrWhiteSpace(county))
            {
                query = query.Where(score => score.County == county);
            }

            var scores = await query
                .OrderByDescending(score => score.TotalPoints)
                .Take(100)
                .Select(score => new
                {
                    score.Id,
                    score.UserId,
                    fullName = $"{score.User!.FirstName} {score.User!.LastName}".Trim(),
                    score.TotalPoints,
                    score.DirectDownlines,
                    score.TasksCompleted,
                    score.VerifiedVoterCards,
                    score.VerificationIntegrityPoints,
                    score.BadgeTier,
                    score.RecognitionTitle,
                    score.IncentiveTag,
                    score.Region,
                    score.County,
                    score.LastCalculatedAt
                })
                .ToListAsync();

            return Ok(scores);
        }

        [HttpGet("my-rank")]
        [Authorize]
        public async Task<ActionResult<object>> GetMyRank([FromQuery] string scope = "National")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var ordered = await _context.LeaderboardScores
                .Where(score => score.Scope == scope)
                .OrderByDescending(score => score.TotalPoints)
                .Select(score => new
                {
                    score.UserId,
                    score.TotalPoints,
                    score.BadgeTier,
                    score.RecognitionTitle,
                    score.IncentiveTag,
                    score.Region,
                    score.County
                })
                .ToListAsync();

            var index = ordered.FindIndex(item => item.UserId == userId);
            if (index < 0)
            {
                return NotFound(new { message = "User does not have a leaderboard score yet." });
            }

            var me = ordered[index];
            return Ok(new
            {
                rank = index + 1,
                totalParticipants = ordered.Count,
                me.TotalPoints,
                me.BadgeTier,
                me.RecognitionTitle,
                me.IncentiveTag,
                me.Region,
                me.County
            });
        }
    }
}
