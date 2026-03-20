using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.LeadershipAccess)]
    public class AuditController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuditController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("events")]
        public async Task<ActionResult<object>> Events([FromQuery] string? eventType = null, [FromQuery] int take = 100)
        {
            var query = _context.AuditLogEvents.AsQueryable();
            if (!string.IsNullOrWhiteSpace(eventType))
            {
                query = query.Where(item => item.EventType == eventType);
            }

            var items = await query
                .OrderByDescending(item => item.CreatedAt)
                .Take(Math.Clamp(take, 1, 500))
                .ToListAsync();

            return Ok(items);
        }
    }
}
