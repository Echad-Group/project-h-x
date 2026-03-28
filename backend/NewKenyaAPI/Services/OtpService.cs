using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using System.Security.Cryptography;
using System.Text;

namespace NewKenyaAPI.Services
{
    public class OtpService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<OtpService> _logger;

        public OtpService(ApplicationDbContext context, IEmailService emailService, ILogger<OtpService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<(bool Success, string Message)> GenerateAndSendOtpAsync(ApplicationUser user, string purpose)
        {
            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            var activeCodes = await _context.OtpVerificationCodes
                .Where(o => o.UserId == user.Id && o.Purpose == purpose && !o.IsUsed)
                .ToListAsync();

            foreach (var item in activeCodes)
            {
                item.IsUsed = true;
                item.UsedAt = DateTime.UtcNow;
            }

            var otp = new OtpVerificationCode
            {
                UserId = user.Id,
                Purpose = purpose,
                CodeHash = HashCode(code),
                ExpiresAt = expiresAt,
                IsUsed = false,
                Attempts = 0
            };

            _context.OtpVerificationCodes.Add(otp);
            await _context.SaveChangesAsync();

            var recipient = user.Email ?? user.UserName ?? string.Empty;
            await _emailService.SendOtpEmailAsync(recipient, purpose, code);

            _logger.LogInformation("OTP generated for user {UserId} with purpose {Purpose}", user.Id, purpose);
            return (true, "OTP sent successfully.");
        }

        public async Task<(bool Success, string Message)> VerifyOtpAsync(ApplicationUser user, string purpose, string code)
        {
            var now = DateTime.UtcNow;
            var otp = await _context.OtpVerificationCodes
                .Where(o => o.UserId == user.Id && o.Purpose == purpose && !o.IsUsed && o.ExpiresAt >= now)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otp == null)
            {
                return (false, "OTP is missing or expired.");
            }

            otp.Attempts += 1;
            if (otp.Attempts > 5)
            {
                otp.IsUsed = true;
                otp.UsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return (false, "Maximum OTP attempts exceeded.");
            }

            var incomingHash = HashCode(code);
            if (!string.Equals(incomingHash, otp.CodeHash, StringComparison.Ordinal))
            {
                await _context.SaveChangesAsync();
                return (false, "Invalid OTP code.");
            }

            otp.IsUsed = true;
            otp.UsedAt = DateTime.UtcNow;
            user.IsOtpVerified = true;
            user.OtpVerifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "OTP verified successfully.");
        }

        private static string HashCode(string code)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));
            return Convert.ToBase64String(bytes);
        }
    }
}
