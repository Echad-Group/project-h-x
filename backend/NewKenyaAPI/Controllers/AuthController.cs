using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NewKenyaAPI.Models;
using NewKenyaAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly OtpService _otpService;
        private readonly IEmailService _emailService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            OtpService otpService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _otpService = otpService;
            _emailService = emailService;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            var campaignRole = ResolveSelfRegisterRole(request.CampaignRole);

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                CampaignRole = campaignRole,
                NationalIdNumber = request.NationalIdNumber,
                Region = request.Region,
                County = request.County,
                SubCounty = request.SubCounty,
                Constituency = request.Constituency,
                Ward = request.Ward,
                PollingStation = request.PollingStation,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            await _userManager.AddToRoleAsync(user, campaignRole == UserRoles.Volunteer ? UserRoles.Volunteer : UserRoles.User);

            var token = await GenerateJwtTokenAsync(user);
            var expiresAt = DateTime.UtcNow.AddDays(7);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new AuthResponse
            {
                Token = token,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                CampaignRole = user.CampaignRole,
                VerificationStatus = user.VerificationStatus,
                VoterCardStatus = user.VoterCardStatus,
                Region = user.Region,
                County = user.County,
                OtpVerified = user.IsOtpVerified,
                Roles = roles.ToList(),
                ExpiresAt = expiresAt
            });
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            if (!user.IsOtpVerified && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                await _otpService.GenerateAndSendOtpAsync(user, OtpPurposes.Login);
                return Unauthorized(new { message = "OTP verification required.", otpRequired = true, email = user.Email });
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var token = await GenerateJwtTokenAsync(user);
            var expiresAt = DateTime.UtcNow.AddDays(7);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new AuthResponse
            {
                Token = token,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                CampaignRole = user.CampaignRole,
                VerificationStatus = user.VerificationStatus,
                VoterCardStatus = user.VoterCardStatus,
                Region = user.Region,
                County = user.County,
                OtpVerified = user.IsOtpVerified,
                Roles = roles.ToList(),
                ExpiresAt = expiresAt
            });
        }

        // POST: api/Auth/send-otp
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Ok(new { message = "If the account exists, OTP has been sent." });
            }

            var result = await _otpService.GenerateAndSendOtpAsync(user, request.Purpose);
            return Ok(new { message = result.Message });
        }

        // POST: api/Auth/verify
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid verification request." });
            }

            var verify = await _otpService.VerifyOtpAsync(user, request.Purpose, request.Code);
            if (!verify.Success)
            {
                return BadRequest(new { message = verify.Message });
            }

            return Ok(new { message = verify.Message, level = request.Level, verifiedAt = user.OtpVerifiedAt });
        }

        // POST: api/Auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully" });
        }

        // POST: api/Auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user doesn't exist
                return Ok(new { message = "If the email exists, a password reset link has been sent." });
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            
            if (!result.Succeeded)
            {
                return BadRequest(new { 
                    message = "Failed to reset password. The reset link may have expired.",
                    errors = result.Errors.Select(e => e.Description) 
                });
            }

            // Mark email as confirmed if they're setting password from welcome email
            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }

            return Ok(new { message = "Password has been reset successfully" });
        }

        // POST: api/Auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            
            if (user == null)
            {
                // Don't reveal that the user doesn't exist
                return Ok(new { message = "If the email exists, a password reset link has been sent." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendPasswordResetEmailAsync(user.Email!, token);
            
            return Ok(new { 
                message = "Password reset instructions have been sent to your email.",
                // Only include token in development for testing
                #if DEBUG
                token = token,
                email = user.Email
                #endif
            });
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("firstName", user.FirstName ?? ""),
                new Claim("lastName", user.LastName ?? ""),
                new Claim("campaignRole", user.CampaignRole ?? UserRoles.User),
                new Claim("verificationStatus", user.VerificationStatus ?? CampaignVerificationStatuses.Pending),
                new Claim("voterCardStatus", user.VoterCardStatus ?? CampaignVoterCardStatuses.Missing)
            };

            if (!string.IsNullOrWhiteSpace(user.Region))
            {
                claims.Add(new Claim("region", user.Region));
            }

            if (!string.IsNullOrWhiteSpace(user.County))
            {
                claims.Add(new Claim("county", user.County));
            }

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "NewKenyaAPI",
                audience: _configuration["Jwt:Audience"] ?? "NewKenyaApp",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string ResolveSelfRegisterRole(string? requestedRole)
        {
            if (string.Equals(requestedRole, UserRoles.Volunteer, StringComparison.OrdinalIgnoreCase))
            {
                return UserRoles.Volunteer;
            }

            return UserRoles.User;
        }
    }
}
