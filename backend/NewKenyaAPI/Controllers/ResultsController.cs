using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using System.Security.Claims;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("submit")]
        [Authorize(Roles = UserRoles.LeadershipAccess + "," + UserRoles.PollingStationAgent)]
        public async Task<ActionResult<object>> Submit([FromBody] ElectionResultSubmitRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var todayWindow = DateTime.UtcNow.ToString("yyyyMMdd");
            var duplicate = await _context.ElectionResults.AnyAsync(result =>
                result.SubmittedByUserId == userId &&
                result.ReportingWindow == todayWindow &&
                result.PollingStationCode == request.PollingStationCode);

            if (duplicate)
            {
                return Conflict(new { message = "Duplicate submission detected for this polling station in the current window." });
            }

            var totalVotes = request.CandidateA + request.CandidateB + request.CandidateC + request.RejectedVotes;
            var status = totalVotes > request.RegisteredVoters
                ? ElectionResultStatuses.PendingValidation
                : ElectionResultStatuses.Validated;

            var notes = totalVotes > request.RegisteredVoters
                ? "Outlier detected: total votes exceed registered voters."
                : "Validated by automatic consistency checks.";

            var entity = new ElectionResult
            {
                SubmittedByUserId = userId,
                PollingStationCode = request.PollingStationCode,
                Constituency = request.Constituency,
                County = request.County,
                Region = request.Region,
                CandidateA = request.CandidateA,
                CandidateB = request.CandidateB,
                CandidateC = request.CandidateC,
                RejectedVotes = request.RejectedVotes,
                RegisteredVoters = request.RegisteredVoters,
                Status = status,
                ValidationNotes = notes,
                ReportingWindow = todayWindow,
                SubmittedAt = DateTime.UtcNow,
                ValidatedAt = status == ElectionResultStatuses.Validated ? DateTime.UtcNow : null
            };

            _context.ElectionResults.Add(entity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Result submitted.", entity.Id, entity.Status, entity.ValidationNotes });
        }

        [HttpGet("aggregate")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> Aggregate([FromQuery] string? reportingWindow = null, [FromQuery] string? county = null)
        {
            var window = string.IsNullOrWhiteSpace(reportingWindow) ? DateTime.UtcNow.ToString("yyyyMMdd") : reportingWindow;

            var query = _context.ElectionResults.Where(result => result.ReportingWindow == window && result.Status != ElectionResultStatuses.Rejected);
            if (!string.IsNullOrWhiteSpace(county))
            {
                query = query.Where(result => result.County == county);
            }

            var summary = await query
                .GroupBy(result => new { result.ReportingWindow, result.County })
                .Select(group => new
                {
                    group.Key.ReportingWindow,
                    group.Key.County,
                    candidateA = group.Sum(result => result.CandidateA),
                    candidateB = group.Sum(result => result.CandidateB),
                    candidateC = group.Sum(result => result.CandidateC),
                    rejectedVotes = group.Sum(result => result.RejectedVotes),
                    stationsReported = group.Select(result => result.PollingStationCode).Distinct().Count(),
                    pendingValidation = group.Count(result => result.Status == ElectionResultStatuses.PendingValidation)
                })
                .OrderBy(item => item.County)
                .ToListAsync();

            return Ok(summary);
        }

        [HttpGet("status")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> Status([FromQuery] string? reportingWindow = null)
        {
            var window = string.IsNullOrWhiteSpace(reportingWindow) ? DateTime.UtcNow.ToString("yyyyMMdd") : reportingWindow;
            var query = _context.ElectionResults.Where(result => result.ReportingWindow == window);

            var response = new
            {
                reportingWindow = window,
                submitted = await query.CountAsync(),
                validated = await query.CountAsync(result => result.Status == ElectionResultStatuses.Validated),
                pending = await query.CountAsync(result => result.Status == ElectionResultStatuses.PendingValidation),
                rejected = await query.CountAsync(result => result.Status == ElectionResultStatuses.Rejected)
            };

            return Ok(response);
        }
    }
}
