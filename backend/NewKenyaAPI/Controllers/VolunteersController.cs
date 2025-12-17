using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using NewKenyaAPI.Services;
using System.Security.Claims;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public VolunteersController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
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
        public async Task<ActionResult<object>> PostVolunteer(VolunteerCreateRequest request)
        {
            // Get the current user ID if authenticated
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Check if email already exists as volunteer
            var existingVolunteer = await _context.Volunteers
                .FirstOrDefaultAsync(v => v.Email == request.Email);

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
                                volunteer = existingVolunteer,
                                accountCreated = false
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
            }

            // Create volunteer record
            var volunteer = new Volunteer
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                City = request.City,
                Region = request.Region,
                AvailabilityZones = request.AvailabilityZones,
                Skills = request.Skills,
                HoursPerWeek = request.HoursPerWeek,
                AvailableWeekends = request.AvailableWeekends,
                AvailableEvenings = request.AvailableEvenings,
                Interests = request.Interests,
                UserId = userId
            };

            bool accountCreated = false;
            string? temporaryPassword = null;

            // If user is NOT authenticated, create a user account automatically
            if (string.IsNullOrEmpty(userId))
            {
                // Check if user account already exists with this email
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                
                if (existingUser != null)
                {
                    // User account exists but they're not logged in
                    // Link volunteer to existing account
                    volunteer.UserId = existingUser.Id;
                    accountCreated = false;
                }
                else
                {
                    // Create new user account
                    var nameParts = request.Name.Split(' ', 2);
                    var firstName = nameParts.Length > 0 ? nameParts[0] : request.Name;
                    var lastName = nameParts.Length > 1 ? nameParts[1] : "";
                    
                    // Generate temporary password
                    temporaryPassword = GenerateTemporaryPassword();
                    
                    var newUser = new ApplicationUser
                    {
                        UserName = request.Email,
                        Email = request.Email,
                        FirstName = firstName,
                        LastName = lastName,
                        EmailConfirmed = false, // They need to verify email
                        CreatedAt = DateTime.UtcNow
                    };

                    var result = await _userManager.CreateAsync(newUser, temporaryPassword);
                    
                    if (result.Succeeded)
                    {
                        // Assign Volunteer role
                        await _userManager.AddToRoleAsync(newUser, UserRoles.Volunteer);
                        
                        // Generate password reset token for welcome email
                        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(newUser);
                        
                        volunteer.UserId = newUser.Id;
                        accountCreated = true;
                        
                        // Send welcome email with password reset link
                        try
                        {
                            await _emailService.SendVolunteerWelcomeEmailAsync(
                                newUser.Email!, 
                                request.Name, 
                                resetToken
                            );
                        }
                        catch (Exception ex)
                        {
                            // Log but don't fail registration if email fails
                            Console.WriteLine($"Failed to send welcome email: {ex.Message}");
                        }
                    }
                    else
                    {
                        return BadRequest(new 
                        { 
                            message = "Failed to create user account",
                            errors = result.Errors.Select(e => e.Description)
                        });
                    }
                }
            }

            _context.Volunteers.Add(volunteer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVolunteer), new { id = volunteer.Id }, new
            {
                volunteer = volunteer,
                accountCreated = accountCreated,
                message = accountCreated 
                    ? "Welcome! Your volunteer account has been created. Check your email for login instructions."
                    : "You've been registered as a volunteer!",
                // Only include temporary password in development for testing
                #if DEBUG
                temporaryPassword = temporaryPassword
                #endif
            });
        }

        private string GenerateTemporaryPassword()
        {
            // Generate a secure random password: 12 characters with mixed case, digits, and special chars
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*";
            
            var random = new Random();
            var password = new char[12];
            
            // Ensure at least one of each type
            password[0] = upperChars[random.Next(upperChars.Length)];
            password[1] = lowerChars[random.Next(lowerChars.Length)];
            password[2] = digits[random.Next(digits.Length)];
            password[3] = special[random.Next(special.Length)];
            
            // Fill remaining with random mix
            var allChars = upperChars + lowerChars + digits + special;
            for (int i = 4; i < 12; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }
            
            // Shuffle the password
            for (int i = password.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (password[i], password[j]) = (password[j], password[i]);
            }
            
            return new string(password);
        }

        public class VolunteerCreateRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public string? City { get; set; }
            public string? Region { get; set; }
            public string? AvailabilityZones { get; set; }
            public string? Skills { get; set; }
            public int? HoursPerWeek { get; set; }
            public bool AvailableWeekends { get; set; }
            public bool AvailableEvenings { get; set; }
            public string? Interests { get; set; }
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
