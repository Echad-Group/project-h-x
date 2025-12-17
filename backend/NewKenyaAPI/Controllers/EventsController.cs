using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents([FromQuery] bool includeUnpublished = false)
        {
            var query = _context.Events.AsQueryable();

            if (!includeUnpublished)
            {
                query = query.Where(e => e.IsPublished);
            }

            var events = await query
                .OrderBy(e => e.Date)
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return Ok(eventItem);
        }

        // GET: api/Events/upcoming
        [HttpGet("upcoming")]
        public async Task<ActionResult<IEnumerable<Event>>> GetUpcomingEvents([FromQuery] int limit = 10)
        {
            var upcomingEvents = await _context.Events
                .Where(e => e.IsPublished && e.Date > DateTime.UtcNow)
                .OrderBy(e => e.Date)
                .Take(limit)
                .ToListAsync();

            return Ok(upcomingEvents);
        }

        // GET: api/Events/past
        [HttpGet("past")]
        public async Task<ActionResult<IEnumerable<Event>>> GetPastEvents([FromQuery] int limit = 10)
        {
            var pastEvents = await _context.Events
                .Where(e => e.IsPublished && e.Date <= DateTime.UtcNow)
                .OrderByDescending(e => e.Date)
                .Take(limit)
                .ToListAsync();

            return Ok(pastEvents);
        }

        // POST: api/Events
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<Event>> CreateEvent(Event eventItem)
        {
            eventItem.CreatedAt = DateTime.UtcNow;
            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = eventItem.Id }, eventItem);
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UpdateEvent(int id, Event eventItem)
        {
            if (id != eventItem.Id)
            {
                return BadRequest();
            }

            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            existingEvent.Title = eventItem.Title;
            existingEvent.Description = eventItem.Description;
            existingEvent.Date = eventItem.Date;
            existingEvent.Location = eventItem.Location;
            existingEvent.City = eventItem.City;
            existingEvent.Region = eventItem.Region;
            existingEvent.ImageUrl = eventItem.ImageUrl;
            existingEvent.Type = eventItem.Type;
            existingEvent.Capacity = eventItem.Capacity;
            existingEvent.IsPublished = eventItem.IsPublished;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Events/5/rsvps
        [HttpGet("{id}/rsvps")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<IEnumerable<EventRSVP>>> GetEventRSVPs(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            var rsvps = await _context.EventRSVPs
                .Where(r => r.EventId == id.ToString())
                .ToListAsync();

            return Ok(rsvps);
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
