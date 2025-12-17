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
    public class AssignmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Assignments/volunteer/5
        [HttpGet("volunteer/{volunteerId}")]
        public async Task<ActionResult<object>> GetVolunteerAssignments(int volunteerId)
        {
            var assignments = await _context.VolunteerAssignments
                .Include(va => va.Unit)
                .Include(va => va.Team)
                .Where(va => va.VolunteerId == volunteerId && va.IsActive)
                .Select(va => new
                {
                    va.Id,
                    va.AssignedAt,
                    Unit = new
                    {
                        va.Unit.Id,
                        va.Unit.Name,
                        va.Unit.Description,
                        va.Unit.Icon,
                        va.Unit.Color,
                        va.Unit.TelegramLink,
                        va.Unit.WhatsAppLink
                    },
                    Team = va.Team != null ? new
                    {
                        va.Team.Id,
                        va.Team.Name,
                        va.Team.Description,
                        va.Team.Icon,
                        va.Team.TelegramLink,
                        va.Team.WhatsAppLink
                    } : null
                })
                .ToListAsync();

            return Ok(assignments);
        }

        // POST: api/Assignments
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<VolunteerAssignment>> AssignVolunteer([FromBody] AssignmentCreateRequest request)
        {
            var volunteer = await _context.Volunteers.FindAsync(request.VolunteerId);
            if (volunteer == null)
            {
                return BadRequest(new { message = "Volunteer not found" });
            }

            var unit = await _context.Units.FindAsync(request.UnitId);
            if (unit == null)
            {
                return BadRequest(new { message = "Unit not found" });
            }

            if (request.TeamId.HasValue)
            {
                var team = await _context.Teams.FindAsync(request.TeamId.Value);
                if (team == null || team.UnitId != request.UnitId)
                {
                    return BadRequest(new { message = "Team not found or doesn't belong to this unit" });
                }
            }

            // Check if already assigned
            var existing = await _context.VolunteerAssignments
                .FirstOrDefaultAsync(va => 
                    va.VolunteerId == request.VolunteerId && 
                    va.UnitId == request.UnitId && 
                    va.TeamId == request.TeamId &&
                    va.IsActive);

            if (existing != null)
            {
                return BadRequest(new { message = "Volunteer already assigned to this unit/team" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var assignment = new VolunteerAssignment
            {
                VolunteerId = request.VolunteerId,
                UnitId = request.UnitId,
                TeamId = request.TeamId,
                Notes = request.Notes,
                AssignedByUserId = userId,
                IsActive = true
            };

            _context.VolunteerAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            // Return projected result to avoid circular references
            var result = new
            {
                assignment.Id,
                assignment.VolunteerId,
                assignment.UnitId,
                assignment.TeamId,
                assignment.Notes,
                assignment.AssignedByUserId,
                assignment.AssignedAt,
                assignment.IsActive
            };

            return CreatedAtAction(nameof(GetVolunteerAssignments), new { volunteerId = request.VolunteerId }, result);
        }

        // PUT: api/Assignments/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAssignment(int id, [FromBody] AssignmentUpdateRequest request)
        {
            var assignment = await _context.VolunteerAssignments.FindAsync(id);
            
            if (assignment == null)
            {
                return NotFound();
            }

            if (request.TeamId.HasValue)
            {
                var team = await _context.Teams.FindAsync(request.TeamId.Value);
                if (team == null || team.UnitId != assignment.UnitId)
                {
                    return BadRequest(new { message = "Team not found or doesn't belong to this unit" });
                }
                assignment.TeamId = request.TeamId.Value;
            }

            assignment.Notes = request.Notes ?? assignment.Notes;
            assignment.IsActive = request.IsActive ?? assignment.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Assignments/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveAssignment(int id)
        {
            var assignment = await _context.VolunteerAssignments.FindAsync(id);
            
            if (assignment == null)
            {
                return NotFound();
            }

            assignment.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Assignments/bulk
        [HttpPost("bulk")]
        [Authorize]
        public async Task<ActionResult<object>> BulkAssign([FromBody] BulkAssignmentRequest request)
        {
            var unit = await _context.Units.FindAsync(request.UnitId);
            if (unit == null)
            {
                return BadRequest(new { message = "Unit not found" });
            }

            if (request.TeamId.HasValue)
            {
                var team = await _context.Teams.FindAsync(request.TeamId.Value);
                if (team == null || team.UnitId != request.UnitId)
                {
                    return BadRequest(new { message = "Team not found or doesn't belong to this unit" });
                }
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int successCount = 0;
            int skipCount = 0;

            foreach (var volunteerId in request.VolunteerIds)
            {
                // Check if already assigned
                var existing = await _context.VolunteerAssignments
                    .FirstOrDefaultAsync(va => 
                        va.VolunteerId == volunteerId && 
                        va.UnitId == request.UnitId && 
                        va.TeamId == request.TeamId &&
                        va.IsActive);

                if (existing != null)
                {
                    skipCount++;
                    continue;
                }

                var assignment = new VolunteerAssignment
                {
                    VolunteerId = volunteerId,
                    UnitId = request.UnitId,
                    TeamId = request.TeamId,
                    Notes = request.Notes,
                    AssignedByUserId = userId,
                    IsActive = true
                };

                _context.VolunteerAssignments.Add(assignment);
                successCount++;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Bulk assignment completed",
                successCount,
                skipCount,
                totalRequested = request.VolunteerIds.Count
            });
        }
    }

    // Request models
    public class AssignmentCreateRequest
    {
        public int VolunteerId { get; set; }
        public int UnitId { get; set; }
        public int? TeamId { get; set; }
        public string? Notes { get; set; }
    }

    public class AssignmentUpdateRequest
    {
        public int? TeamId { get; set; }
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }

    public class BulkAssignmentRequest
    {
        public List<int> VolunteerIds { get; set; } = new List<int>();
        public int UnitId { get; set; }
        public int? TeamId { get; set; }
        public string? Notes { get; set; }
    }
}
