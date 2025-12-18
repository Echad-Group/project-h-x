using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewKenyaAPI.Models;
using NewKenyaAPI.Models.DTOs;
using System.Security.Claims;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/UserProfile
        [HttpGet]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var profile = new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                Location = user.Location,
                Website = user.Website,
                Twitter = user.Twitter,
                Facebook = user.Facebook,
                LinkedIn = user.LinkedIn,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Roles = roles.ToList()
            };

            return Ok(profile);
        }

        // PUT: api/UserProfile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Clean up empty strings to null for optional URL field
            if (string.IsNullOrWhiteSpace(dto.Website))
            {
                dto.Website = null;
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Bio = dto.Bio;
            user.Location = dto.Location;
            user.Website = dto.Website;
            user.Twitter = dto.Twitter;
            user.Facebook = dto.Facebook;
            user.LinkedIn = dto.LinkedIn;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            return NoContent();
        }

        // PUT: api/UserProfile/password
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            return Ok(new { message = "Password changed successfully" });
        }

        // PUT: api/UserProfile/email
        [HttpPut("email")]
        public async Task<IActionResult> UpdateEmail(UpdateEmailDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Verify password
            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
            {
                return BadRequest(new { message = "Invalid password" });
            }

            // Check if email is already taken
            var existingUser = await _userManager.FindByEmailAsync(dto.NewEmail);
            if (existingUser != null && existingUser.Id != userId)
            {
                return BadRequest(new { message = "Email is already in use" });
            }

            var result = await _userManager.SetEmailAsync(user, dto.NewEmail);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            // Update username to match email
            await _userManager.SetUserNameAsync(user, dto.NewEmail);

            return Ok(new { message = "Email updated successfully" });
        }

        // DELETE: api/UserProfile
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount([FromBody] string password)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Verify password before deleting
            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordValid)
            {
                return BadRequest(new { message = "Invalid password" });
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            return Ok(new { message = "Account deleted successfully" });
        }
    }
}
