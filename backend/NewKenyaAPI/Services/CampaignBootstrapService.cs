using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using Task = System.Threading.Tasks.Task;

namespace NewKenyaAPI.Services
{
    public class CampaignBootstrapService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CampaignBootstrapService> _logger;

        public CampaignBootstrapService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<CampaignBootstrapService> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task SeedInitialHierarchyAsync(string defaultPassword)
        {
            var hq = await EnsureUserAsync("hq@newkenya.org", "HQ", "Commander", UserRoles.SuperAdmin, defaultPassword, region: "National");
            var regional = await EnsureUserAsync("region.nairobi@newkenya.org", "Nairobi", "Regional", UserRoles.RegionalLeader, defaultPassword, region: "Nairobi");
            var county = await EnsureUserAsync("county.nairobi@newkenya.org", "Nairobi", "County", UserRoles.CountyLeader, defaultPassword, region: "Nairobi", county: "Nairobi");
            var ward = await EnsureUserAsync("ward.westlands@newkenya.org", "Westlands", "Ward", UserRoles.WardLeader, defaultPassword, region: "Nairobi", county: "Nairobi", ward: "Westlands");
            var volunteer = await EnsureUserAsync("volunteer.westlands@newkenya.org", "Westlands", "Volunteer", UserRoles.Volunteer, defaultPassword, region: "Nairobi", county: "Nairobi", ward: "Westlands");

            await LinkParentAsync(regional, hq);
            await LinkParentAsync(county, regional);
            await LinkParentAsync(ward, county);
            await LinkParentAsync(volunteer, ward);

            await RecalculateDownlineCountsAsync();
            await _context.SaveChangesAsync();
        }

        private async Task<ApplicationUser> EnsureUserAsync(
            string email,
            string firstName,
            string lastName,
            string role,
            string password,
            string? region = null,
            string? county = null,
            string? ward = null)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    CampaignRole = role,
                    VerificationStatus = CampaignVerificationStatuses.Verified,
                    VoterCardStatus = CampaignVoterCardStatuses.Verified,
                    Region = region,
                    County = county,
                    Ward = ward,
                    IsOtpVerified = true,
                    OtpVerifiedAt = DateTime.UtcNow,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var create = await _userManager.CreateAsync(user, password);
                if (!create.Succeeded)
                {
                    _logger.LogWarning("Unable to create seed user {Email}: {Errors}", email, string.Join(", ", create.Errors.Select(e => e.Description)));
                    user = await _userManager.FindByEmailAsync(email) ?? throw new InvalidOperationException("Seed user missing");
                }
            }

            if (!await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            user.CampaignRole = role;
            user.Region = region ?? user.Region;
            user.County = county ?? user.County;
            user.Ward = ward ?? user.Ward;
            await _userManager.UpdateAsync(user);
            return user;
        }

        private static Task LinkParentAsync(ApplicationUser child, ApplicationUser parent)
        {
            child.ParentUserId = parent.Id;
            return Task.CompletedTask;
        }

        private async Task RecalculateDownlineCountsAsync()
        {
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                user.DownlineCount = users.Count(candidate => candidate.ParentUserId == user.Id);
            }
        }
    }
}
