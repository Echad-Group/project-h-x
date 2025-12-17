using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using NewKenyaAPI.Models.DTOs;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventRSVPsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventRSVPsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EventRSVPs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventRSVP>>> GetEventRSVPs([FromQuery] int? eventId)
        {
            var query = _context.EventRSVPs.AsQueryable();

            if (eventId.HasValue)
            {
                query = query.Where(e => e.EventId == eventId.Value);
            }

            return await query
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        // GET: api/EventRSVPs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventRSVP>> GetEventRSVP(int id)
        {
            var eventRSVP = await _context.EventRSVPs.FindAsync(id);

            if (eventRSVP == null)
            {
                return NotFound();
            }

            return eventRSVP;
        }

        // GET: api/EventRSVPs/count/{eventId}
        [HttpGet("count/{eventId}")]
        public async Task<ActionResult<object>> GetEventRSVPCount(int eventId)
        {
            var count = await _context.EventRSVPs
                .Where(e => e.EventId == eventId)
                .CountAsync();

            var totalGuests = await _context.EventRSVPs
                .Where(e => e.EventId == eventId)
                .SumAsync(e => e.NumberOfGuests);

            return new
            {
                rsvpCount = count,
                totalAttendees = totalGuests
            };
        }

        // POST: api/EventRSVPs
        [HttpPost]
        public async Task<ActionResult<EventRSVP>> PostEventRSVP(CreateEventRSVPDto dto)
        {
            // Check if the event exists
            var eventExists = await _context.Events.AnyAsync(e => e.Id == dto.EventId);
            if (!eventExists)
            {
                return BadRequest(new { message = "Event not found." });
            }

            // Check if user already RSVPd for this event
            var existingRSVP = await _context.EventRSVPs
                .FirstOrDefaultAsync(e => e.EventId == dto.EventId && e.Email == dto.Email);

            if (existingRSVP != null)
            {
                return Conflict(new { message = "You have already RSVPed for this event." });
            }

            var eventRSVP = new EventRSVP
            {
                EventId = dto.EventId,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                NumberOfGuests = dto.NumberOfGuests,
                SpecialRequirements = dto.SpecialRequirements,
                CreatedAt = DateTime.UtcNow
            };

            _context.EventRSVPs.Add(eventRSVP);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventRSVP), new { id = eventRSVP.Id }, eventRSVP);
        }

        // PUT: api/EventRSVPs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEventRSVP(int id, EventRSVP eventRSVP)
        {
            if (id != eventRSVP.Id)
            {
                return BadRequest();
            }

            _context.Entry(eventRSVP).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventRSVPExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/EventRSVPs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventRSVP(int id)
        {
            var eventRSVP = await _context.EventRSVPs.FindAsync(id);
            if (eventRSVP == null)
            {
                return NotFound();
            }

            _context.EventRSVPs.Remove(eventRSVP);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> EventRSVPExists(int id)
        {
            return await _context.EventRSVPs.AnyAsync(e => e.Id == id);
        }
    }
}
