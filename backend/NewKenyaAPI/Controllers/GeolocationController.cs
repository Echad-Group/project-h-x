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
    public class GeolocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GeolocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("ingest")]
        public async Task<ActionResult<object>> Ingest([FromBody] CampaignGeoPingRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            if (!request.ConsentGranted)
            {
                return BadRequest(new { message = "Explicit geolocation consent is required." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var ping = new CampaignGeoPing
            {
                UserId = userId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                AccuracyMeters = request.AccuracyMeters,
                Purpose = string.IsNullOrWhiteSpace(request.Purpose) ? "Coverage" : request.Purpose.Trim(),
                ConsentGranted = request.ConsentGranted,
                IsAnonymized = request.IsAnonymized,
                Region = request.IsAnonymized ? null : user.Region,
                County = request.IsAnonymized ? null : user.County,
                CapturedAt = DateTime.UtcNow
            };

            _context.CampaignGeoPings.Add(ping);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Geo ping captured.", ping.Id, ping.CapturedAt });
        }

        [HttpGet("coverage")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> Coverage([FromQuery] int hours = 24)
        {
            var since = DateTime.UtcNow.AddHours(-Math.Clamp(hours, 1, 168));
            var query = _context.CampaignGeoPings.Where(ping => ping.CapturedAt >= since);

            var regionCoverage = await query
                .GroupBy(ping => ping.Region)
                .Select(group => new
                {
                    region = group.Key ?? "Anonymized",
                    pings = group.Count(),
                    uniqueUsers = group.Select(ping => ping.UserId).Distinct().Count()
                })
                .OrderByDescending(item => item.pings)
                .ToListAsync();

            var countyCoverage = await query
                .GroupBy(ping => new { ping.Region, ping.County })
                .Select(group => new
                {
                    region = group.Key.Region ?? "Anonymized",
                    county = group.Key.County ?? "Anonymized",
                    pings = group.Count(),
                    uniqueUsers = group.Select(ping => ping.UserId).Distinct().Count()
                })
                .OrderByDescending(item => item.pings)
                .Take(200)
                .ToListAsync();

            return Ok(new
            {
                hours,
                totalPings = await query.CountAsync(),
                regionCoverage,
                countyCoverage
            });
        }
    }
}
