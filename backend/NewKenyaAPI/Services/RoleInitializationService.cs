using Microsoft.AspNetCore.Identity;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Services
{
    public class RoleInitializationService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RoleInitializationService> _logger;

        public RoleInitializationService(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<RoleInitializationService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async System.Threading.Tasks.Task InitializeRolesAsync()
        {
            try
            {
                // Create roles if they don't exist
                string[] roles = { UserRoles.Admin, UserRoles.Volunteer, UserRoles.User, UserRoles.Moderator, UserRoles.TeamLead };

                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        var result = await _roleManager.CreateAsync(new IdentityRole(role));
                        if (result.Succeeded)
                        {
                            _logger.LogInformation($"Role '{role}' created successfully");
                        }
                        else
                        {
                            _logger.LogError($"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }
                }

                _logger.LogInformation("Role initialization completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing roles");
            }
        }

        public async Task<bool> CreateDefaultAdminAsync(string email, string password)
        {
            try
            {
                var adminUser = await _userManager.FindByEmailAsync(email);
                
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = "System",
                        LastName = "Administrator",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    var result = await _userManager.CreateAsync(adminUser, password);
                    
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
                        _logger.LogInformation($"Default admin user created: {email}");
                        return true;
                    }
                    else
                    {
                        _logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        return false;
                    }
                }
                else
                {
                    // Ensure existing admin has the Admin role
                    if (!await _userManager.IsInRoleAsync(adminUser, UserRoles.Admin))
                    {
                        await _userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
                        _logger.LogInformation($"Admin role added to existing user: {email}");
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating default admin");
                return false;
            }
        }
    }
}
