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
            var resetUrl = $"{_configuration["AppSettings:FrontendUrl"]}/reset-password?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(email)}";
            
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
        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🇰🇪 Welcome to New Kenya Movement!</h1>
        </div>
        <div class='content'>
            <h2>Hello {name},</h2>
            <p>Thank you for registering as a volunteer! We're excited to have you join our movement to build a better Kenya.</p>
            
            <p>We've created an account for you so you can access your volunteer dashboard, view your assignments, and connect with your team.</p>
            
            <p><strong>Next Step:</strong> Set your password to activate your account.</p>
            
            <p style='text-align: center;'>
                <a href='{resetUrl}' class='button'>Set Your Password</a>
            </p>
            
            <p>Or copy and paste this link into your browser:<br>
            <a href='{resetUrl}'>{resetUrl}</a></p>
            
            <p>This link will expire in 24 hours for security reasons.</p>
            
            <p><strong>What's Next?</strong></p>
            <ul>
                <li>Set your password using the link above</li>
                <li>Log in to your volunteer dashboard</li>
                <li>View your unit and team assignments</li>
                <li>Join communication channels (WhatsApp/Telegram)</li>
                <li>Start making a difference!</li>
            </ul>
            
            <p>If you didn't sign up to be a volunteer, please ignore this email.</p>
            
            <p>Asante sana!<br>
            The New Kenya Team</p>
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
            var resetUrl = $"{_configuration["AppSettings:FrontendUrl"]}/reset-password?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(email)}";
            
            var subject = "Reset Your Password - New Kenya Movement";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2>Password Reset Request</h2>
        <p>You requested to reset your password. Click the button below to set a new password:</p>
        <p style='text-align: center; margin: 30px 0;'>
            <a href='{resetUrl}' style='background-color: #16A34A; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                Reset Password
            </a>
        </p>
        <p>Or copy this link: <a href='{resetUrl}'>{resetUrl}</a></p>
        <p>This link will expire in 24 hours.</p>
        <p>If you didn't request this, please ignore this email.</p>
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
