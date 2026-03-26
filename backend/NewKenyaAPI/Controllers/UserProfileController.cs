using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                NationalIdNumber = user.NationalIdNumber,
                VoterCardNumber = user.VoterCardNumber,
                IdImageUrl = user.IdImageUrl,
                SelfieImageUrl = user.SelfieImageUrl,
                VoterCardImageUrl = user.VoterCardImageUrl,
                VerificationStatus = user.VerificationStatus,
                VoterCardStatus = user.VoterCardStatus,
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

            var normalizedNationalId = string.IsNullOrWhiteSpace(dto.NationalIdNumber) ? null : dto.NationalIdNumber.Trim();
            var normalizedVoterCard = string.IsNullOrWhiteSpace(dto.VoterCardNumber) ? null : dto.VoterCardNumber.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedNationalId))
            {
                var duplicateNationalId = await _userManager.Users.AnyAsync(u => u.Id != user.Id && u.NationalIdNumber == normalizedNationalId);
                if (duplicateNationalId)
                {
                    return Conflict(new { message = "An account with this National ID number already exists." });
                }
            }

            if (!string.IsNullOrWhiteSpace(normalizedVoterCard))
            {
                var duplicateVoterCard = await _userManager.Users.AnyAsync(u => u.Id != user.Id && u.VoterCardNumber == normalizedVoterCard);
                if (duplicateVoterCard)
                {
                    return Conflict(new { message = "An account with this voter card number already exists." });
                }
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
            user.ProfilePhotoUrl = dto.ProfilePhotoUrl;
            user.NationalIdNumber = normalizedNationalId;
            user.VoterCardNumber = normalizedVoterCard;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            return NoContent();
        }

        // POST: api/UserProfile/upload-verification-document
        [HttpPost("upload-verification-document")]
        public async Task<IActionResult> UploadVerificationDocument([FromForm] IFormFile file, [FromForm] string documentType)
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

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
            }

            var normalizedType = documentType?.Trim().ToLowerInvariant();
            var validTypes = new[] { "nida", "voter-card", "selfie" };
            if (string.IsNullOrWhiteSpace(normalizedType) || !validTypes.Contains(normalizedType))
            {
                return BadRequest(new { message = "Invalid document type. Allowed values: nida, voter-card, selfie." });
            }

            if (file.Length > 10 * 1024 * 1024)
            {
                return BadRequest(new { message = "File size must be less than 10MB" });
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = normalizedType == "selfie"
                ? new[] { ".jpg", ".jpeg", ".png", ".webp" }
                : new[] { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { message = "Invalid file type for selected document" });
            }

            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "verification-docs");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string? currentUrl = normalizedType switch
                {
                    "nida" => user.IdImageUrl,
                    "voter-card" => user.VoterCardImageUrl,
                    "selfie" => user.SelfieImageUrl,
                    _ => null
                };

                if (!string.IsNullOrWhiteSpace(currentUrl))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", currentUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                var fileName = $"{userId}_{normalizedType}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileUrl = $"/uploads/verification-docs/{fileName}";

                switch (normalizedType)
                {
                    case "nida":
                        user.IdImageUrl = fileUrl;
                        break;
                    case "voter-card":
                        user.VoterCardImageUrl = fileUrl;
                        user.VoterCardStatus = CampaignVoterCardStatuses.Pending;
                        break;
                    case "selfie":
                        user.SelfieImageUrl = fileUrl;
                        break;
                }

                user.VerificationStatus = CampaignVerificationStatuses.Pending;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
                }

                return Ok(new
                {
                    message = "Verification document uploaded successfully",
                    documentType = normalizedType,
                    fileUrl,
                    verificationStatus = user.VerificationStatus,
                    voterCardStatus = user.VoterCardStatus
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading verification document", error = ex.Message });
            }
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

        // POST: api/UserProfile/upload-photo
        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile file)
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

            // Validate file
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
            }

            // Validate file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { message = "File size must be less than 5MB" });
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { message = "Invalid file type. Allowed: JPG, PNG, GIF, WEBP" });
            }

            try
            {
                // Delete old photo if exists
                if (!string.IsNullOrEmpty(user.ProfilePhotoUrl))
                {
                    var oldPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePhotoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPhotoPath))
                    {
                        System.IO.File.Delete(oldPhotoPath);
                    }
                }

                // Generate unique filename
                var fileName = $"{userId}_{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile-photos");
                
                // Ensure directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Update user profile
                var photoUrl = $"/uploads/profile-photos/{fileName}";
                user.ProfilePhotoUrl = photoUrl;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    // Clean up file if database update fails
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
                }

                return Ok(new { photoUrl = photoUrl, message = "Profile photo uploaded successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading file", error = ex.Message });
            }
        }

        // DELETE: api/UserProfile/photo
        [HttpDelete("photo")]
        public async Task<IActionResult> DeleteProfilePhoto()
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

            if (string.IsNullOrEmpty(user.ProfilePhotoUrl))
            {
                return BadRequest(new { message = "No profile photo to delete" });
            }

            try
            {
                // Delete file from disk
                var photoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePhotoUrl.TrimStart('/'));
                if (System.IO.File.Exists(photoPath))
                {
                    System.IO.File.Delete(photoPath);
                }

                // Update user profile
                user.ProfilePhotoUrl = null;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
                }

                return Ok(new { message = "Profile photo deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting file", error = ex.Message });
            }
        }
    }
}
