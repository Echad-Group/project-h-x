using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using NewKenyaAPI.Services;

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
                    score.Region,
                    score.County,
                    score.LastCalculatedAt
                })
                .ToListAsync();

            return Ok(scores);
        }
    }
}
