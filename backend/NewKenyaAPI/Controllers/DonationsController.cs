using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DonationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Donations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Donation>>> GetDonations()
        {
            return await _context.Donations
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        // GET: api/Donations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Donation>> GetDonation(int id)
        {
            var donation = await _context.Donations.FindAsync(id);

            if (donation == null)
            {
                return NotFound();
            }

            return donation;
        }

        // GET: api/Donations/stats
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetDonationStats()
        {
            var totalAmount = await _context.Donations.SumAsync(d => d.Amount);
            var donorCount = await _context.Donations
                .Select(d => d.DonorEmail)
                .Distinct()
                .CountAsync();
            var recentDonations = await _context.Donations
                .OrderByDescending(d => d.CreatedAt)
                .Take(5)
                .ToListAsync();

            return new
            {
                totalAmount,
                donorCount,
                recentDonations
            };
        }

        // POST: api/Donations
        [HttpPost]
        public async Task<ActionResult<Donation>> PostDonation(Donation donation)
        {
            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDonation), new { id = donation.Id }, donation);
        }

        // DELETE: api/Donations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonation(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
            {
                return NotFound();
            }

            _context.Donations.Remove(donation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
