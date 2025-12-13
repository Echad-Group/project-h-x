namespace NewKenyaAPI.Services
{
    public interface IEmailService
    {
        Task SendVolunteerWelcomeEmailAsync(string email, string name, string resetToken);
        Task SendPasswordResetEmailAsync(string email, string resetToken);
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }
}
