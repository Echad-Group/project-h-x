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

        // GET: api/Events/slug/kisumu-townhall
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<Event>> GetEventBySlug(string slug)
        {
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.Slug == slug && e.IsPublished);

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
            eventItem.Slug = GenerateSlug(eventItem.Title);
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
            existingEvent.Slug = GenerateSlug(eventItem.Title);
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
                .Where(r => r.EventId == id)
                .ToListAsync();

            return Ok(rsvps);
        }

        // POST: api/Events/seed
        [HttpPost("seed")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> SeedEvents()
        {
            // Check if events already exist
            if (await _context.Events.AnyAsync())
            {
                return BadRequest(new { message = "Events already exist. Clear the database first if you want to reseed." });
            }

            var events = new List<Event>
            {
                new Event
                {
                    Title = "Townhall - Kisumu",
                    Slug = "kisumu-townhall",
                    Description = "Join us for a community townhall to discuss jobs, youth engagement, and local development priorities.",
                    Date = new DateTime(2025, 11, 12, 14, 0, 0, DateTimeKind.Utc),
                    Location = "Kisumu Stadium",
                    City = "Kisumu",
                    Region = "Nyanza",
                    ImageUrl = "https://images.unsplash.com/photo-1540910419892-4a36d2c3266c?w=800",
                    Type = "Townhall",
                    Capacity = 5000,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Event
                {
                    Title = "Youth Summit - Nairobi",
                    Slug = "youth-summit",
                    Description = "A full-day summit focused on youth empowerment, entrepreneurship, and skills development.",
                    Date = new DateTime(2025, 11, 28, 9, 0, 0, DateTimeKind.Utc),
                    Location = "KICC, Nairobi",
                    City = "Nairobi",
                    Region = "Nairobi",
                    ImageUrl = "https://images.unsplash.com/photo-1523240795612-9a054b0db644?w=800",
                    Type = "Summit",
                    Capacity = 8000,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Event
                {
                    Title = "Coastal Roadshow - Mombasa",
                    Slug = "mombasa-roadshow",
                    Description = "Campaign roadshow highlighting coastal development, port expansion, and tourism initiatives.",
                    Date = new DateTime(2025, 12, 5, 10, 0, 0, DateTimeKind.Utc),
                    Location = "Mombasa Grounds",
                    City = "Mombasa",
                    Region = "Coast",
                    ImageUrl = "https://images.unsplash.com/photo-1578575437130-527eed3abbec?w=800",
                    Type = "Roadshow",
                    Capacity = 6000,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Event
                {
                    Title = "Farmers' Forum - Nakuru",
                    Slug = "nakuru-farmers-forum",
                    Description = "Agricultural policy dialogue with farmers focusing on subsidies, market access, and modern farming.",
                    Date = new DateTime(2025, 12, 15, 11, 0, 0, DateTimeKind.Utc),
                    Location = "Nakuru Agricultural Center",
                    City = "Nakuru",
                    Region = "Rift Valley",
                    ImageUrl = "https://images.unsplash.com/photo-1625246333195-78d9c38ad449?w=800",
                    Type = "Forum",
                    Capacity = 4000,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Event
                {
                    Title = "Unity Rally - Eldoret",
                    Slug = "eldoret-rally",
                    Description = "Major campaign rally bringing together communities to celebrate unity and discuss regional development.",
                    Date = new DateTime(2025, 12, 22, 15, 0, 0, DateTimeKind.Utc),
                    Location = "Eldoret Sports Club",
                    City = "Eldoret",
                    Region = "Rift Valley",
                    ImageUrl = "https://images.unsplash.com/photo-1529107386315-e1a2ed48a620?w=800",
                    Type = "Rally",
                    Capacity = 10000,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Event
                {
                    Title = "Women Empowerment Forum - Nairobi",
                    Slug = "women-empowerment",
                    Description = "Forum celebrating women leadership and announcing the Women Empowerment Fund initiatives.",
                    Date = new DateTime(2026, 1, 10, 13, 0, 0, DateTimeKind.Utc),
                    Location = "Serena Hotel, Nairobi",
                    City = "Nairobi",
                    Region = "Nairobi",
                    ImageUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=800",
                    Type = "Forum",
                    Capacity = 2000,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.Events.AddRange(events);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Successfully seeded {events.Count} events." });
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

        private string GenerateSlug(string title)
        {
            return title.ToLowerInvariant()
                .Replace(" - ", "-")
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace(",", "")
                .Replace(".", "");
        }
    }
}
