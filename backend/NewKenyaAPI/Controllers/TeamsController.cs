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
    [Authorize(Roles = UserRoles.Admin)]
    public class TeamsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTeams([FromQuery] int? unitId = null)
        {
            var query = _context.Teams
                .Include(t => t.Unit)
                .OrderBy(t => t.DisplayOrder)
                .AsQueryable();

            if (unitId.HasValue)
            {
                query = query.Where(t => t.UnitId == unitId.Value);
            }

            var teams = await query
                .Where(t => t.IsActive)
                .Select(t => new
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
                    UnitId = t.UnitId,
                    UnitName = t.Unit.Name,
                    UnitIcon = t.Unit.Icon,
                    VolunteerCount = t.VolunteerAssignments.Count(va => va.IsActive),
                    TaskCount = t.Tasks.Count(task => task.Status != "Completed" && task.Status != "Cancelled")
                })
                .ToListAsync();

            return Ok(teams);
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTeam(int id)
        {
            var team = await _context.Teams
                .Include(t => t.Unit)
                .Include(t => t.VolunteerAssignments.Where(va => va.IsActive))
                    .ThenInclude(va => va.Volunteer)
                .Include(t => t.Tasks.Where(task => task.Status != "Completed" && task.Status != "Cancelled"))
                .Where(t => t.Id == id && t.IsActive)
                .Select(t => new
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
                    t.CreatedAt,
                    Unit = new
                    {
                        t.Unit.Id,
                        t.Unit.Name,
                        t.Unit.Icon,
                        t.Unit.Color
                    },
                    Volunteers = t.VolunteerAssignments.Select(va => new
                    {
                        va.VolunteerId,
                        va.Volunteer.Name,
                        va.Volunteer.Email,
                        va.Volunteer.Phone,
                        va.Volunteer.City,
                        va.Volunteer.Region,
                        va.Volunteer.Skills,
                        va.AssignedAt
                    }),
                    Tasks = t.Tasks.Select(task => new
                    {
                        task.Id,
                        task.Title,
                        task.Description,
                        task.Status,
                        task.Priority,
                        task.DueDate,
                        task.Location,
                        task.Region
                    })
                })
                .FirstOrDefaultAsync();

            if (team == null)
            {
                return NotFound();
            }

            return Ok(team);
        }

        // POST: api/Teams
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Team>> CreateTeam([FromBody] TeamCreateRequest request)
        {
            var unit = await _context.Units.FindAsync(request.UnitId);
            if (unit == null)
            {
                return BadRequest(new { message = "Unit not found" });
            }

            var team = new Team
            {
                Name = request.Name,
                Description = request.Description,
                Icon = request.Icon,
                UnitId = request.UnitId,
                DisplayOrder = request.DisplayOrder,
                RequiredSkills = request.RequiredSkills,
                PreferredLocations = request.PreferredLocations,
                TelegramLink = request.TelegramLink,
                WhatsAppLink = request.WhatsAppLink,
                IsActive = true
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }

        // PUT: api/Teams/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTeam(int id, [FromBody] TeamUpdateRequest request)
        {
            var team = await _context.Teams.FindAsync(id);
            
            if (team == null)
            {
                return NotFound();
            }

            team.Name = request.Name ?? team.Name;
            team.Description = request.Description ?? team.Description;
            team.Icon = request.Icon;
            team.DisplayOrder = request.DisplayOrder ?? team.DisplayOrder;
            team.RequiredSkills = request.RequiredSkills;
            team.PreferredLocations = request.PreferredLocations;
            team.TelegramLink = request.TelegramLink;
            team.WhatsAppLink = request.WhatsAppLink;
            team.IsActive = request.IsActive ?? team.IsActive;
            team.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            
            if (team == null)
            {
                return NotFound();
            }

            // Soft delete
            team.IsActive = false;
            team.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // Request models
    public class TeamCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int UnitId { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public string? RequiredSkills { get; set; }
        public string? PreferredLocations { get; set; }
        public string? TelegramLink { get; set; }
        public string? WhatsAppLink { get; set; }
    }

    public class TeamUpdateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public int? DisplayOrder { get; set; }
        public string? RequiredSkills { get; set; }
        public string? PreferredLocations { get; set; }
        public string? TelegramLink { get; set; }
        public string? WhatsAppLink { get; set; }
        public bool? IsActive { get; set; }
    }
}
