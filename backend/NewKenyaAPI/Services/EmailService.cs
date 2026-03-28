using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace NewKenyaAPI.Services
{
    public class EmailService : IEmailService
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
        private static readonly SemaphoreSlim ArchiveLock = new(1, 1);

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, IWebHostEnvironment environment, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _environment = environment;
            _logger = logger;
        }

        public async Task SendVolunteerWelcomeEmailAsync(string email, string name, string resetToken)
        {
            var webResetUrl = BuildWebResetUrl(email, resetToken);
            var mobileResetUrl = BuildMobileLink("reset-password", ("token", resetToken), ("email", email));

            var subject = "Welcome to New Kenya Movement - Set Your Password";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #16A34A; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 20px; }}
        .button {{ 
            display: inline-block; 
            background-color: #16A34A; 
            color: white; 
            padding: 12px 30px; 
            text-decoration: none; 
            border-radius: 5px;
            margin: 20px 0;
        }}
        .button-secondary {{
            background-color: #1F2937;
        }}
        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to New Kenya Movement</h1>
        </div>
        <div class='content'>
            <h2>Hello {WebUtility.HtmlEncode(name)},</h2>
            <p>Thank you for registering as a volunteer. We have created your account and you can activate it from the mobile app or the web portal.</p>
            <p><strong>Next step:</strong> set your password to activate your account.</p>
            <p style='text-align: center;'>
                <a href='{mobileResetUrl}' class='button'>Open In Mobile App</a>
            </p>
            <p style='text-align: center;'>
                <a href='{webResetUrl}' class='button button-secondary'>Use Web Browser Instead</a>
            </p>
            <p>Mobile deep link:<br><a href='{mobileResetUrl}'>{mobileResetUrl}</a></p>
            <p>Web fallback:<br><a href='{webResetUrl}'>{webResetUrl}</a></p>
            <p>This link will expire in 24 hours for security reasons.</p>
            <p>If you did not sign up to be a volunteer, you can ignore this email.</p>
            <p>Asante sana,<br>The New Kenya Team</p>
        </div>
        <div class='footer'>
            <p>New Kenya Movement | Building a Better Future Together</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            var webResetUrl = BuildWebResetUrl(email, resetToken);
            var mobileResetUrl = BuildMobileLink("reset-password", ("token", resetToken), ("email", email));

            var subject = "Reset Your Password - New Kenya Movement";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2>Password Reset Request</h2>
        <p>You requested to reset your password. Open the mobile app directly or use the browser fallback below.</p>
        <p style='text-align: center; margin: 30px 0;'>
            <a href='{mobileResetUrl}' style='background-color: #16A34A; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                Open In Mobile App
            </a>
        </p>
        <p style='text-align: center; margin: 20px 0;'>
            <a href='{webResetUrl}' style='background-color: #1F2937; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                Use Web Browser Instead
            </a>
        </p>
        <p>Mobile deep link: <a href='{mobileResetUrl}'>{mobileResetUrl}</a></p>
        <p>Web fallback: <a href='{webResetUrl}'>{webResetUrl}</a></p>
        <p>This link will expire in 24 hours.</p>
        <p>If you did not request this, please ignore this email.</p>
    </div>
</body>
</html>";

            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendOtpEmailAsync(string email, string purpose, string code)
        {
            var mobileOtpUrl = BuildMobileLink("otp", ("email", email), ("purpose", purpose), ("code", code));
            var subject = "Your New Kenya OTP Code";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2>Your One-Time Password</h2>
        <p>Use the following code to continue your {WebUtility.HtmlEncode(purpose)} flow:</p>
        <p style='font-size: 30px; font-weight: bold; letter-spacing: 6px; color: #111827;'>{WebUtility.HtmlEncode(code)}</p>
        <p style='text-align: center; margin: 30px 0;'>
            <a href='{mobileOtpUrl}' style='background-color: #16A34A; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                Open In Mobile App
            </a>
        </p>
        <p>Mobile deep link: <a href='{mobileOtpUrl}'>{mobileOtpUrl}</a></p>
        <p>This code expires in 10 minutes.</p>
    </div>
</body>
</html>";

            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@newkenya.org";
            var fromName = _configuration["EmailSettings:FromName"] ?? "New Kenya Movement";
            var archiveRecord = new ArchivedEmailRecord
            {
                SentAtUtc = DateTime.UtcNow,
                To = to,
                Subject = subject,
                HtmlBody = htmlBody,
                FromEmail = fromEmail,
                FromName = fromName,
                SmtpConfigured = !string.IsNullOrEmpty(smtpHost) && !string.IsNullOrEmpty(smtpUsername),
                DeliveryStatus = "pending"
            };

            // If email is not configured, log instead of sending
            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername))
            {
                archiveRecord.DeliveryStatus = "skipped";
                archiveRecord.Error = "SMTP not configured";
                await AppendToArchiveAsync(archiveRecord);

                _logger.LogWarning("Email service not configured. Email would be sent to: {Email}", to);
                _logger.LogInformation("Subject: {Subject}", subject);
                _logger.LogInformation("Body (first 200 chars): {Body}", htmlBody.Substring(0, Math.Min(200, htmlBody.Length)));
                return;
            }

            try
            {
                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                archiveRecord.DeliveryStatus = "sent";
                await AppendToArchiveAsync(archiveRecord);
                _logger.LogInformation("Email sent successfully to: {Email}", to);
            }
            catch (Exception ex)
            {
                archiveRecord.DeliveryStatus = "failed";
                archiveRecord.Error = ex.Message;
                await AppendToArchiveAsync(archiveRecord);
                _logger.LogError(ex, "Failed to send email to: {Email}", to);
                // Don't throw - email failure shouldn't break the registration flow
            }
        }

        private string BuildWebResetUrl(string email, string resetToken)
        {
            var frontendUrl = (_configuration["AppSettings:FrontendUrl"] ?? "http://localhost:5173").TrimEnd('/');
            return $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(email)}";
        }

        private string BuildMobileLink(string path, params (string Key, string Value)[] queryParameters)
        {
            var linkBase = _configuration["AppSettings:MobileDeepLinkBase"] ?? "projecthx://";
            if (!linkBase.EndsWith('/') && !linkBase.EndsWith("://", StringComparison.Ordinal))
            {
                linkBase += "/";
            }

            var query = string.Join("&", queryParameters.Select(parameter => $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value)}"));
            var basePath = $"{linkBase}{path}";
            return string.IsNullOrWhiteSpace(query)
                ? basePath
                : $"{basePath}?{query}";
        }

        private async Task AppendToArchiveAsync(ArchivedEmailRecord record)
        {
            var archivePath = Path.Combine(_environment.ContentRootPath, "App_Data", "email-archive.json");
            var archiveDirectory = Path.GetDirectoryName(archivePath);

            if (!string.IsNullOrWhiteSpace(archiveDirectory))
            {
                Directory.CreateDirectory(archiveDirectory);
            }

            await ArchiveLock.WaitAsync();
            try
            {
                List<ArchivedEmailRecord> archive;

                if (File.Exists(archivePath))
                {
                    await using var readStream = File.OpenRead(archivePath);
                    archive = await JsonSerializer.DeserializeAsync<List<ArchivedEmailRecord>>(readStream) ?? new List<ArchivedEmailRecord>();
                }
                else
                {
                    archive = new List<ArchivedEmailRecord>();
                }

                archive.Add(record);

                await using var writeStream = File.Create(archivePath);
                await JsonSerializer.SerializeAsync(writeStream, archive, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to archive email to JSON file.");
            }
            finally
            {
                ArchiveLock.Release();
            }
        }

        private sealed class ArchivedEmailRecord
        {
            public DateTime SentAtUtc { get; set; }
            public string To { get; set; } = string.Empty;
            public string Subject { get; set; } = string.Empty;
            public string HtmlBody { get; set; } = string.Empty;
            public string FromEmail { get; set; } = string.Empty;
            public string FromName { get; set; } = string.Empty;
            public bool SmtpConfigured { get; set; }
            public string DeliveryStatus { get; set; } = string.Empty;
            public string? Error { get; set; }
        }
    }
}
