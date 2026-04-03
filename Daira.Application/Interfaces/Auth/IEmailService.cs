namespace Daira.Application.Interfaces.Auth
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(string email, string confirmationLink);
        Task SendEmailAsync(string to, string subject, string body);
        Task SendPasswordResetAsync(string email, string resetLink);
    }
}
