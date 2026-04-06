using System.Net;
using System.Net.Mail;

namespace SenegaleseAssociation.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["Email:SmtpUsername"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@senegalese-mn.org";
            var fromName = _configuration["Email:FromName"] ?? "Senegalese Association of Minnesota";

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
            {
                throw new InvalidOperationException("Email configuration is missing. Please configure SMTP settings in appsettings.json");
            }

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendContactNotificationAsync(string name, string email, string subject, string message)
        {
            var adminEmail = _configuration["Email:AdminEmail"] ?? "admin@senegalese-mn.org";

            if (string.IsNullOrEmpty(adminEmail))
            {
                throw new InvalidOperationException("Admin email is not configured in appsettings.json");
            }

            var emailBody = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #00A86B 0%, #228B22 100%); color: white; padding: 20px; border-radius: 5px 5px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
                        .field {{ margin-bottom: 15px; }}
                        .field-label {{ font-weight: bold; color: #00A86B; }}
                        .field-value {{ margin-top: 5px; padding: 10px; background: white; border-left: 3px solid #00A86B; }}
                        .footer {{ background: #333; color: white; padding: 15px; text-align: center; border-radius: 0 0 5px 5px; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2 style='margin: 0;'>New Contact Message Received</h2>
                        </div>
                        <div class='content'>
                            <div class='field'>
                                <div class='field-label'>From:</div>
                                <div class='field-value'>{name}</div>
                            </div>
                            <div class='field'>
                                <div class='field-label'>Email:</div>
                                <div class='field-value'><a href='mailto:{email}'>{email}</a></div>
                            </div>
                            <div class='field'>
                                <div class='field-label'>Subject:</div>
                                <div class='field-value'>{subject}</div>
                            </div>
                            <div class='field'>
                                <div class='field-label'>Message:</div>
                                <div class='field-value'>{message.Replace("\n", "<br>")}</div>
                            </div>
                            <div class='field'>
                                <div class='field-label'>Received At:</div>
                                <div class='field-value'>{DateTime.Now:MMMM dd, yyyy 'at' hh:mm tt}</div>
                            </div>
                        </div>
                        <div class='footer'>
                            <p>This email was sent automatically from your website's contact form.</p>
                            <p>Senegalese Association of Minnesota &copy; {DateTime.Now.Year}</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(adminEmail, $"New Contact Message: {subject}", emailBody);
        }
    }
}
