namespace SenegaleseAssociation.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task SendContactNotificationAsync(string name, string email, string subject, string message);
    }
}
