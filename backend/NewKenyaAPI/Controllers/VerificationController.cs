using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewKenyaAPI.Models;
using NewKenyaAPI.Services;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin + "," + UserRoles.RegionalLeader + "," + UserRoles.CountyLeader)]
    public class VerificationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly VerificationReviewService _verificationReviewService;
        private readonly AuditLogService _auditLogService;

        public VerificationController(
            UserManager<ApplicationUser> userManager,
            VerificationReviewService verificationReviewService,
            AuditLogService auditLogService)
        {
            _userManager = userManager;
            _verificationReviewService = verificationReviewService;
            _auditLogService = auditLogService;
        }

        [HttpGet("queue")]
        public ActionResult<List<VerificationQueueItem>> GetQueue([FromQuery] string? status = null)
        {
            var normalizedStatus = NormalizeVerificationStatus(status);

            var users = _userManager.Users
                .Where(user => user.VerificationStatus == normalizedStatus)
                .OrderBy(user => user.CreatedAt)
                .ToList();

            var response = users.Select(MapQueueItem).ToList();
            return Ok(response);
        }

        [HttpGet("queue/{userId}")]
        public async Task<ActionResult<VerificationQueueItem>> GetQueueItem(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(MapQueueItem(user));
        }

        [HttpPost("queue/{userId}/decision")]
        public async Task<ActionResult<VerificationQueueItem>> Decide(string userId, [FromBody] VerificationDecisionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var decision = NormalizeDecision(request.Decision);
            user.VerificationStatus = decision == "Approved"
                ? CampaignVerificationStatuses.Verified
                : CampaignVerificationStatuses.Rejected;
            user.VoterCardStatus = decision == "Approved"
                ? CampaignVoterCardStatuses.Verified
                : CampaignVoterCardStatuses.Rejected;
            user.VerificationReviewedAt = DateTime.UtcNow;

            var update = await _userManager.UpdateAsync(user);
            if (!update.Succeeded)
            {
                return BadRequest(new { errors = update.Errors.Select(e => e.Description) });
            }

            var reviewerName = User.FindFirstValue("firstName") ?? User.FindFirstValue(ClaimTypes.Name) ?? "Reviewer";
            _verificationReviewService.AddEvent(user.Id, new VerificationTimelineEvent
            {
                Timestamp = DateTime.UtcNow,
                Action = decision,
                ReviewerName = reviewerName,
                Notes = request.ReviewerNotes?.Trim()
            });

            await _auditLogService.WriteAsync(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                "VerificationDecision",
                "ApplicationUser",
                user.Id,
                new { decision, notes = request.ReviewerNotes },
                "VerificationController.Decide");

            return Ok(MapQueueItem(user));
        }

        [HttpGet("queue/{userId}/document/{documentType}")]
        public async Task<IActionResult> GetDocument(string userId, string documentType)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var relativePath = ResolveDocumentPath(user, documentType);
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return NotFound(new { message = "Document not found." });
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "secure-storage", relativePath.Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound(new { message = "Document file not found." });
            }

            var encryptedBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            var decryptedBytes = DecryptPayload(encryptedBytes, ResolveIdentityEncryptionKey());
            return File(decryptedBytes, "application/octet-stream");
        }

        private VerificationQueueItem MapQueueItem(ApplicationUser user)
        {
            return new VerificationQueueItem
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = string.Join(" ", new[] { user.FirstName, user.LastName }.Where(part => !string.IsNullOrWhiteSpace(part))),
                VerificationStatus = user.VerificationStatus,
                VoterCardStatus = user.VoterCardStatus,
                CampaignRole = user.CampaignRole,
                Region = user.Region,
                County = user.County,
                NidaDocumentUrl = $"/api/verification/queue/{user.Id}/document/nida",
                VoterCardDocumentUrl = $"/api/verification/queue/{user.Id}/document/voter-card",
                SelfieDocumentUrl = $"/api/verification/queue/{user.Id}/document/selfie",
                CreatedAt = user.CreatedAt,
                ReviewedAt = user.VerificationReviewedAt,
                Timeline = _verificationReviewService.GetTimeline(user.Id)
            };
        }

        private static string? ResolveDocumentPath(ApplicationUser user, string documentType)
        {
            var key = documentType.Trim().ToLowerInvariant();
            return key switch
            {
                "nida" => user.IdImageUrl,
                "voter-card" => user.VoterCardImageUrl,
                "selfie" => user.SelfieImageUrl,
                _ => null
            };
        }

        private static string NormalizeVerificationStatus(string? status)
        {
            var value = (status ?? string.Empty).Trim().ToLowerInvariant();
            return value switch
            {
                "verified" => CampaignVerificationStatuses.Verified,
                "rejected" => CampaignVerificationStatuses.Rejected,
                _ => CampaignVerificationStatuses.Pending
            };
        }

        private static string NormalizeDecision(string? decision)
        {
            var value = (decision ?? string.Empty).Trim().ToLowerInvariant();
            return value switch
            {
                "approve" => "Approved",
                "approved" => "Approved",
                "reject" => "Rejected",
                "rejected" => "Rejected",
                _ => "Rejected"
            };
        }

        private byte[] ResolveIdentityEncryptionKey()
        {
            var configured = HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()["IdentityDocs:EncryptionKey"];

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

            var fallbackSecret = HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()["Jwt:SecretKey"] ?? "NewKenyaIdentityFallbackSecretKeyMaterial";
            return SHA256.HashData(Encoding.UTF8.GetBytes(fallbackSecret));
        }

        private static byte[] DecryptPayload(byte[] encryptedBytes, byte[] key)
        {
            var iv = encryptedBytes[..16];
            var cipher = encryptedBytes[16..];

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            return decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
        }
    }
}
