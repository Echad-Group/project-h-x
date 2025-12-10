using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Units
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUnits([FromQuery] bool includeInactive = false)
        {
            var query = _context.Units
                .Include(u => u.Teams.Where(t => t.IsActive || includeInactive))
                .OrderBy(u => u.DisplayOrder)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(u => u.IsActive);
            }

            var units = await query
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Description,
                    u.Icon,
                    u.Color,
                    u.DisplayOrder,
                    u.TelegramLink,
                    u.WhatsAppLink,
                    TeamCount = u.Teams.Count,
                    VolunteerCount = u.VolunteerAssignments.Count(va => va.IsActive),
                    Teams = u.Teams.Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Description,
                        t.Icon,
                        t.DisplayOrder,
                        t.RequiredSkills,
                        t.PreferredLocations,
                        t.TelegramLink,
                        t.WhatsAppLink,
                        VolunteerCount = t.VolunteerAssignments.Count(va => va.IsActive)
                    }).OrderBy(t => t.DisplayOrder)
                })
                .ToListAsync();

            return Ok(units);
        }

        // GET: api/Units/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUnit(int id)
        {
            var unit = await _context.Units
                .Include(u => u.Teams.Where(t => t.IsActive))
                .Include(u => u.VolunteerAssignments.Where(va => va.IsActive))
                    .ThenInclude(va => va.Volunteer)
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Description,
                    u.Icon,
                    u.Color,
                    u.DisplayOrder,
                    u.TelegramLink,
                    u.WhatsAppLink,
                    u.CreatedAt,
                    Teams = u.Teams.Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Description,
                        t.Icon,
                        t.DisplayOrder,
                        t.RequiredSkills,
                        t.PreferredLocations,
                        t.TelegramLink,
                        t.WhatsAppLink,
                        VolunteerCount = t.VolunteerAssignments.Count(va => va.IsActive)
                    }).OrderBy(t => t.DisplayOrder),
                    Volunteers = u.VolunteerAssignments.Select(va => new
                    {
                        va.VolunteerId,
                        va.Volunteer.Name,
                        va.Volunteer.Email,
                        va.Volunteer.City,
                        va.Volunteer.Region,
                        va.Volunteer.Skills,
                        TeamId = va.TeamId,
                        TeamName = va.Team != null ? va.Team.Name : null,
                        va.AssignedAt
                    })
                })
                .FirstOrDefaultAsync();

            if (unit == null)
            {
                return NotFound();
            }

            return Ok(unit);
        }

        // POST: api/Units
        [HttpPost]
        [Authorize] // Only authenticated users can create units
        public async Task<ActionResult<Unit>> CreateUnit([FromBody] UnitCreateRequest request)
        {
            var unit = new Unit
            {
                Name = request.Name,
                Description = request.Description,
                Icon = request.Icon,
                Color = request.Color,
                DisplayOrder = request.DisplayOrder,
                TelegramLink = request.TelegramLink,
                WhatsAppLink = request.WhatsAppLink,
                IsActive = true
            };

            _context.Units.Add(unit);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUnit), new { id = unit.Id }, unit);
        }

        // PUT: api/Units/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUnit(int id, [FromBody] UnitUpdateRequest request)
        {
            var unit = await _context.Units.FindAsync(id);
            
            if (unit == null)
            {
                return NotFound();
            }

            unit.Name = request.Name ?? unit.Name;
            unit.Description = request.Description ?? unit.Description;
            unit.Icon = request.Icon;
            unit.Color = request.Color;
            unit.DisplayOrder = request.DisplayOrder ?? unit.DisplayOrder;
            unit.TelegramLink = request.TelegramLink;
            unit.WhatsAppLink = request.WhatsAppLink;
            unit.IsActive = request.IsActive ?? unit.IsActive;
            unit.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Units/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            
            if (unit == null)
            {
                return NotFound();
            }

            // Soft delete
            unit.IsActive = false;
            unit.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Units/5/volunteers
        [HttpGet("{id}/volunteers")]
        public async Task<ActionResult<IEnumerable<object>>> GetUnitVolunteers(int id, [FromQuery] int? teamId = null)
        {
            var query = _context.VolunteerAssignments
                .Include(va => va.Volunteer)
                .Include(va => va.Team)
                .Where(va => va.UnitId == id && va.IsActive);

            if (teamId.HasValue)
            {
                query = query.Where(va => va.TeamId == teamId.Value);
            }

            var volunteers = await query
                .Select(va => new
                {
                    va.VolunteerId,
                    va.Volunteer.Name,
                    va.Volunteer.Email,
                    va.Volunteer.Phone,
                    va.Volunteer.City,
                    va.Volunteer.Region,
                    va.Volunteer.Skills,
                    va.Volunteer.HoursPerWeek,
                    va.Volunteer.AvailableWeekends,
                    va.Volunteer.AvailableEvenings,
                    TeamId = va.TeamId,
                    TeamName = va.Team != null ? va.Team.Name : null,
                    va.AssignedAt
                })
                .OrderBy(v => v.Name)
                .ToListAsync();

            return Ok(volunteers);
        }
    }

    // Request models
    public class UnitCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public string? TelegramLink { get; set; }
        public string? WhatsAppLink { get; set; }
    }

    public class UnitUpdateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public int? DisplayOrder { get; set; }
        public string? TelegramLink { get; set; }
        public string? WhatsAppLink { get; set; }
        public bool? IsActive { get; set; }
    }
}
