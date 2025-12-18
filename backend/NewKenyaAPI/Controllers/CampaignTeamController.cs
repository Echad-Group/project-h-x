using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignTeamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CampaignTeamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CampaignTeam
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampaignTeamMember>>> GetCampaignTeamMembers()
        {
            return await _context.CampaignTeamMembers
                .Where(m => m.IsActive)
                .OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }

        // GET: api/CampaignTeam/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CampaignTeamMember>> GetCampaignTeamMember(int id)
        {
            var member = await _context.CampaignTeamMembers.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // POST: api/CampaignTeam
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CampaignTeamMember>> PostCampaignTeamMember(CampaignTeamMember member)
        {
            member.CreatedAt = DateTime.UtcNow;
            member.JoinedDate = member.JoinedDate == default ? DateTime.UtcNow : member.JoinedDate;
            
            _context.CampaignTeamMembers.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCampaignTeamMember), new { id = member.Id }, member);
        }

        // PUT: api/CampaignTeam/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampaignTeamMember(int id, CampaignTeamMember member)
        {
            if (id != member.Id)
            {
                return BadRequest();
            }

            var existingMember = await _context.CampaignTeamMembers.FindAsync(id);
            if (existingMember == null)
            {
                return NotFound();
            }

            // Update fields
            existingMember.Name = member.Name;
            existingMember.Role = member.Role;
            existingMember.Email = member.Email;
            existingMember.Phone = member.Phone;
            existingMember.Bio = member.Bio;
            existingMember.Responsibilities = member.Responsibilities;
            existingMember.PhotoUrl = member.PhotoUrl;
            existingMember.TwitterHandle = member.TwitterHandle;
            existingMember.LinkedInUrl = member.LinkedInUrl;
            existingMember.FacebookUrl = member.FacebookUrl;
            existingMember.DisplayOrder = member.DisplayOrder;
            existingMember.IsActive = member.IsActive;
            existingMember.JoinedDate = member.JoinedDate;
            existingMember.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampaignTeamMemberExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/CampaignTeam/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaignTeamMember(int id)
        {
            var member = await _context.CampaignTeamMembers.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.CampaignTeamMembers.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/CampaignTeam/5/reorder
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/reorder")]
        public async Task<IActionResult> ReorderMember(int id, [FromBody] int newOrder)
        {
            var member = await _context.CampaignTeamMembers.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            member.DisplayOrder = newOrder;
            member.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CampaignTeamMemberExists(int id)
        {
            return _context.CampaignTeamMembers.Any(e => e.Id == id);
        }
    }
}
