using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewKenyaAPI.Models;
using NewKenyaAPI.Services;
using System.IO;
using System.Security.Cryptography;
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
        private readonly FaceMatchService _faceMatchService;
        private readonly VerificationReviewService _verificationReviewService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            OtpService otpService,
            IEmailService emailService,
            FaceMatchService faceMatchService,
            VerificationReviewService verificationReviewService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _otpService = otpService;
            _emailService = emailService;
            _faceMatchService = faceMatchService;
            _verificationReviewService = verificationReviewService;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(
            [FromForm] RegisterRequest request,
            IFormFile? nidaDocument,
            IFormFile? voterCardDocument,
            IFormFile? selfieDocument)
        {
            if (nidaDocument == null || voterCardDocument == null || selfieDocument == null)
            {
                return BadRequest(new { message = "NIDA document, voter card document, and selfie are required." });
            }

            if (string.IsNullOrWhiteSpace(request.NationalIdNumber) || string.IsNullOrWhiteSpace(request.VoterCardNumber))
            {
                return BadRequest(new { message = "National ID number and voter card number are required." });
            }

            var normalizedNida = request.NationalIdNumber.Trim();
            var normalizedVoterCard = request.VoterCardNumber.Trim();

            var duplicateNida = await _userManager.Users.AnyAsync(user => user.NationalIdNumber == normalizedNida);
            if (duplicateNida)
            {
                return Conflict(new { message = "An account with this National ID number already exists." });
            }

            var duplicateVoterCard = await _userManager.Users.AnyAsync(user => user.VoterCardNumber == normalizedVoterCard);
            if (duplicateVoterCard)
            {
                return Conflict(new { message = "An account with this voter card number already exists." });
            }

            var selfieHash = await ComputeHashAsync(selfieDocument.OpenReadStream());
            if (await IsDuplicateSelfieAsync(selfieHash))
            {
                return Conflict(new { message = "Duplicate selfie detected. This identity appears to be already registered." });
            }

            var campaignRole = ResolveSelfRegisterRole(request.CampaignRole);

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                CampaignRole = campaignRole,
                NationalIdNumber = normalizedNida,
                VoterCardNumber = normalizedVoterCard,
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

            try
            {
                user.IdImageUrl = await SaveRegistrationDocumentAsync(user.Id, nidaDocument, "nida");
                user.VoterCardImageUrl = await SaveRegistrationDocumentAsync(user.Id, voterCardDocument, "voter-card");
                user.SelfieImageUrl = await SaveRegistrationDocumentAsync(user.Id, selfieDocument, "selfie", selfieHash);
                user.VoterCardStatus = CampaignVoterCardStatuses.Pending;

                var faceMatch = await _faceMatchService.CompareAsync(nidaDocument, selfieDocument);
                user.FaceMatchScore = faceMatch.Score;
                user.VerificationStatus = CampaignVerificationStatuses.Pending;

                _verificationReviewService.AddEvent(user.Id, new VerificationTimelineEvent
                {
                    Timestamp = DateTime.UtcNow,
                    Action = faceMatch.Passed ? "FaceMatchPassed" : "FaceMatchFailed",
                    ReviewerName = "AI Verification Pipeline",
                    Notes = faceMatch.Passed
                        ? $"Automated face-match passed with score {faceMatch.Score}."
                        : $"Automated face-match below threshold with score {faceMatch.Score}; routed to manual review."
                });

                await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to store registration documents.", error = ex.Message });
            }

            await _otpService.GenerateAndSendOtpAsync(user, OtpPurposes.Registration);

            return Ok(new
            {
                email = user.Email,
                otpRequired = true,
                purpose = OtpPurposes.Registration,
                message = "Registration submitted. Enter OTP sent to your email to activate account."
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

            var roles = await _userManager.GetRolesAsync(user);
            var highRiskRoles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                UserRoles.Admin,
                UserRoles.SuperAdmin,
                UserRoles.RegionalLeader,
                UserRoles.CountyLeader,
                UserRoles.SubCountyLeader,
                UserRoles.ConstituencyLeader,
                UserRoles.WardLeader
            };

            var isHighRisk = roles.Any(role => highRiskRoles.Contains(role));
            var otpFreshWindow = DateTime.UtcNow.AddHours(-24);
            var requiresOtpChallenge = !user.IsOtpVerified || (isHighRisk && (!user.OtpVerifiedAt.HasValue || user.OtpVerifiedAt.Value < otpFreshWindow));

            if (requiresOtpChallenge)
            {
                await _otpService.GenerateAndSendOtpAsync(user, OtpPurposes.Login);
                return Unauthorized(new { message = "OTP verification required.", otpRequired = true, email = user.Email });
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var token = await GenerateJwtTokenAsync(user);
            var expiresAt = DateTime.UtcNow.AddDays(7);
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

        private async Task<string> SaveRegistrationDocumentAsync(string userId, IFormFile file, string category, string? fingerprintHash = null)
        {
            const long maxSizeBytes = 8 * 1024 * 1024;
            if (file.Length <= 0 || file.Length > maxSizeBytes)
            {
                throw new InvalidOperationException("Invalid file size. Max allowed is 8MB.");
            }

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "application/pdf" };
            if (!allowedTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Unsupported document format.");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "secure-storage", "identity", category);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext))
            {
                ext = file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) ? ".pdf" : ".jpg";
            }

            var hashSegment = string.IsNullOrWhiteSpace(fingerprintHash)
                ? Guid.NewGuid().ToString("N")
                : fingerprintHash[..Math.Min(16, fingerprintHash.Length)].ToLowerInvariant();

            var fileName = $"{userId}_{category}_{hashSegment}{ext}.enc";
            var fullPath = Path.Combine(uploadsFolder, fileName);

            await using var sourceStream = file.OpenReadStream();
            using var plainBuffer = new MemoryStream();
            await sourceStream.CopyToAsync(plainBuffer);
            var plainBytes = plainBuffer.ToArray();

            var encryptedBytes = EncryptPayload(plainBytes, ResolveIdentityEncryptionKey());
            await System.IO.File.WriteAllBytesAsync(fullPath, encryptedBytes);

            return $"identity/{category}/{fileName}";
        }

        private async Task<bool> IsDuplicateSelfieAsync(string selfieHash)
        {
            var hashPrefix = selfieHash[..Math.Min(16, selfieHash.Length)].ToLowerInvariant();
            return await _userManager.Users.AnyAsync(user =>
                !string.IsNullOrWhiteSpace(user.SelfieImageUrl)
                && user.SelfieImageUrl.Contains(hashPrefix));
        }

        private static async Task<string> ComputeHashAsync(Stream stream)
        {
            stream.Position = 0;
            var bytes = await SHA256.HashDataAsync(stream);
            return Convert.ToHexString(bytes);
        }

        private byte[] ResolveIdentityEncryptionKey()
        {
            var configured = _configuration["IdentityDocs:EncryptionKey"];
            if (!string.IsNullOrWhiteSpace(configured))
            {
                try
                {
                    var key = Convert.FromBase64String(configured);
                    if (key.Length == 32)
                    {
                        return key;
                    }
                }
                catch
                {
                }
            }

            var fallbackSecret = _configuration["Jwt:SecretKey"] ?? "NewKenyaIdentityFallbackSecretKeyMaterial";
            return SHA256.HashData(Encoding.UTF8.GetBytes(fallbackSecret));
        }

        private static byte[] EncryptPayload(byte[] plainBytes, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var output = new byte[aes.IV.Length + cipherBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, output, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherBytes, 0, output, aes.IV.Length, cipherBytes.Length);
            return output;
        }
    }
}
