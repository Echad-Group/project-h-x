using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using System.Security.Claims;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VolunteersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Volunteers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Volunteer>>> GetVolunteers()
        {
            return await _context.Volunteers.ToListAsync();
        }

        // GET: api/Volunteers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Volunteer>> GetVolunteer(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);

            if (volunteer == null)
            {
                return NotFound();
            }

            return volunteer;
        }

        // GET: api/Volunteers/check-status
        [HttpGet("check-status")]
        [Authorize]
        public async Task<ActionResult<object>> CheckVolunteerStatus()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var volunteer = await _context.Volunteers
                .FirstOrDefaultAsync(v => v.UserId == userId);

            if (volunteer != null)
            {
                return Ok(new 
                { 
                    isVolunteer = true,
                    volunteer = new
                    {
                        id = volunteer.Id,
                        name = volunteer.Name,
                        email = volunteer.Email,
                        phone = volunteer.Phone,
                        interests = volunteer.Interests,
                        createdAt = volunteer.CreatedAt
                    }
                });
            }

            return Ok(new { isVolunteer = false });
        }

        // POST: api/Volunteers
        [HttpPost]
        public async Task<ActionResult<Volunteer>> PostVolunteer(Volunteer volunteer)
        {
            // Get the current user ID if authenticated
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Check if email already exists
            var existingVolunteer = await _context.Volunteers
                .FirstOrDefaultAsync(v => v.Email == volunteer.Email);

            if (existingVolunteer != null)
            {
                // If user is authenticated
                if (!string.IsNullOrEmpty(userId))
                {
                    // Get the authenticated user's email to verify it matches
                    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                    
                    // If the volunteer email matches the authenticated user's email
                    if (existingVolunteer.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase))
                    {
                        // If the existing volunteer has no UserId, link it to the current user
                        if (string.IsNullOrEmpty(existingVolunteer.UserId))
                        {
                            existingVolunteer.UserId = userId;
                            await _context.SaveChangesAsync();
                            
                            return Ok(new 
                            { 
                                message = "Your volunteer account has been linked to your user account.",
                                volunteer = existingVolunteer
                            });
                        }
                        // If already linked to this user
                        else if (existingVolunteer.UserId == userId)
                        {
                            return Conflict(new { message = "You are already registered as a volunteer." });
                        }
                        // Linked to a different user (shouldn't happen with proper flow)
                        else
                        {
                            return Conflict(new { message = "This volunteer account is linked to a different user." });
                        }
                    }
                }
                
                // Email exists but doesn't match authenticated user's email (fraud prevention)
                return Conflict(new { message = "A volunteer with this email already exists." });
            }
            
            // If user is authenticated, check if they're already a volunteer by UserId
            if (!string.IsNullOrEmpty(userId))
            {
                var existingUserVolunteer = await _context.Volunteers
                    .FirstOrDefaultAsync(v => v.UserId == userId);
                
                if (existingUserVolunteer != null)
                {
                    return Conflict(new { message = "You are already registered as a volunteer." });
                }
                
                // Link the volunteer to the authenticated user
                volunteer.UserId = userId;
            }

            _context.Volunteers.Add(volunteer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVolunteer), new { id = volunteer.Id }, volunteer);
        }

        // DELETE: api/Volunteers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVolunteer(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            _context.Volunteers.Remove(volunteer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
