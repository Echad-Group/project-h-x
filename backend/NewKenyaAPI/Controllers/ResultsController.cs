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
            var crossSubmitterConflict = await _context.ElectionResults.AnyAsync(result =>
                result.SubmittedByUserId != userId
                && result.ReportingWindow == todayWindow
                && result.PollingStationCode == request.PollingStationCode);

            var integrityScore = ResolveIntegrityConfidence(request.Latitude, request.Longitude, request.DeviceFingerprint);
            var isTamperSuspected = integrityScore < 0.55m;
            var status = totalVotes > request.RegisteredVoters
                ? ElectionResultStatuses.PendingValidation
                : ElectionResultStatuses.Validated;

            if (crossSubmitterConflict || isTamperSuspected)
            {
                status = ElectionResultStatuses.PendingValidation;
            }

            var notes = totalVotes > request.RegisteredVoters
                ? "Outlier detected: total votes exceed registered voters."
                : "Validated by automatic consistency checks.";

            if (crossSubmitterConflict)
            {
                notes = "Conflict detected: multiple submitters reported same polling station/window; pending adjudication.";
            }

            if (isTamperSuspected)
            {
                notes = "Integrity check flagged low confidence metadata; pending manual review.";
            }

            var conflictGroupKey = $"{todayWindow}:{request.PollingStationCode}";

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
                IsConflictFlagged = crossSubmitterConflict,
                ConflictGroupKey = crossSubmitterConflict ? conflictGroupKey : null,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                DeviceFingerprint = request.DeviceFingerprint,
                IntegrityConfidenceScore = integrityScore,
                IsTamperSuspected = isTamperSuspected,
                ReportingWindow = todayWindow,
                SubmittedAt = DateTime.UtcNow,
                ValidatedAt = status == ElectionResultStatuses.Validated ? DateTime.UtcNow : null
            };

            _context.ElectionResults.Add(entity);

            if (crossSubmitterConflict)
            {
                var existingResults = await _context.ElectionResults
                    .Where(result => result.ReportingWindow == todayWindow && result.PollingStationCode == request.PollingStationCode)
                    .ToListAsync();

                foreach (var item in existingResults)
                {
                    item.IsConflictFlagged = true;
                    item.ConflictGroupKey = conflictGroupKey;
                    item.Status = ElectionResultStatuses.PendingValidation;
                    item.ValidationNotes = "Conflict detected with another submitter; pending adjudication.";
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Result submitted.", entity.Id, entity.Status, entity.ValidationNotes });
        }

        [HttpGet("pending-review")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> PendingReview([FromQuery] string? reportingWindow = null)
        {
            var window = string.IsNullOrWhiteSpace(reportingWindow) ? DateTime.UtcNow.ToString("yyyyMMdd") : reportingWindow;
            var items = await _context.ElectionResults
                .Where(result => result.ReportingWindow == window && result.Status == ElectionResultStatuses.PendingValidation)
                .OrderByDescending(result => result.SubmittedAt)
                .Select(result => new
                {
                    result.Id,
                    result.PollingStationCode,
                    result.County,
                    result.Constituency,
                    result.SubmittedByUserId,
                    result.ValidationNotes,
                    result.IsConflictFlagged,
                    result.ConflictGroupKey,
                    result.IntegrityConfidenceScore,
                    result.IsTamperSuspected,
                    result.SubmittedAt
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost("{resultId:int}/review")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> ReviewResult(int resultId, [FromBody] ElectionResultReviewRequest request)
        {
            var reviewerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(reviewerUserId))
            {
                return Unauthorized();
            }

            var result = await _context.ElectionResults.FirstOrDefaultAsync(item => item.Id == resultId);
            if (result == null)
            {
                return NotFound(new { message = "Result not found." });
            }

            var decision = NormalizeReviewDecision(request.Decision);
            result.Status = decision;
            result.ReviewedByUserId = reviewerUserId;
            result.ReviewedAt = DateTime.UtcNow;
            result.ValidationNotes = request.Notes?.Trim() ?? result.ValidationNotes;
            if (decision == ElectionResultStatuses.Validated)
            {
                result.ValidatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Result review decision saved.",
                result.Id,
                result.Status,
                result.ReviewedByUserId,
                result.ReviewedAt
            });
        }

        [HttpGet("conflicts")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> GetConflicts([FromQuery] string? reportingWindow = null)
        {
            var window = string.IsNullOrWhiteSpace(reportingWindow) ? DateTime.UtcNow.ToString("yyyyMMdd") : reportingWindow;

            var conflicts = await _context.ElectionResults
                .Where(result => result.ReportingWindow == window && result.IsConflictFlagged && result.ConflictGroupKey != null)
                .GroupBy(result => result.ConflictGroupKey)
                .Select(group => new
                {
                    conflictGroupKey = group.Key,
                    pollingStationCode = group.Max(item => item.PollingStationCode),
                    submissions = group.Select(item => new
                    {
                        item.Id,
                        item.SubmittedByUserId,
                        item.Status,
                        item.SubmittedAt,
                        item.IntegrityConfidenceScore,
                        item.IsTamperSuspected
                    })
                })
                .ToListAsync();

            return Ok(conflicts);
        }

        [HttpPost("conflicts/{conflictGroupKey}/adjudicate")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> AdjudicateConflict(string conflictGroupKey, [FromBody] ElectionConflictAdjudicationRequest request)
        {
            var reviewerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(reviewerUserId))
            {
                return Unauthorized();
            }

            var conflictResults = await _context.ElectionResults
                .Where(result => result.ConflictGroupKey == conflictGroupKey)
                .ToListAsync();

            if (conflictResults.Count == 0)
            {
                return NotFound(new { message = "Conflict group not found." });
            }

            foreach (var item in conflictResults)
            {
                var isAccepted = item.Id == request.AcceptedResultId;
                item.Status = isAccepted ? ElectionResultStatuses.Validated : ElectionResultStatuses.Rejected;
                item.ValidatedAt = isAccepted ? DateTime.UtcNow : null;
                item.ReviewedByUserId = reviewerUserId;
                item.ReviewedAt = DateTime.UtcNow;
                item.ValidationNotes = request.Notes?.Trim() ?? "Conflict adjudication decision applied.";
                item.IsConflictFlagged = false;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Conflict adjudicated successfully.",
                conflictGroupKey,
                acceptedResultId = request.AcceptedResultId
            });
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

        private static decimal ResolveIntegrityConfidence(decimal? latitude, decimal? longitude, string? deviceFingerprint)
        {
            var score = 0.45m;
            if (latitude.HasValue && longitude.HasValue)
            {
                score += 0.30m;
            }

            if (!string.IsNullOrWhiteSpace(deviceFingerprint))
            {
                score += 0.25m;
            }

            return Math.Round(Math.Min(score, 0.99m), 2, MidpointRounding.AwayFromZero);
        }

        private static string NormalizeReviewDecision(string? decision)
        {
            var value = (decision ?? string.Empty).Trim().ToLowerInvariant();
            return value switch
            {
                "validated" => ElectionResultStatuses.Validated,
                "approve" => ElectionResultStatuses.Validated,
                "approved" => ElectionResultStatuses.Validated,
                _ => ElectionResultStatuses.Rejected
            };
        }
    }
}
