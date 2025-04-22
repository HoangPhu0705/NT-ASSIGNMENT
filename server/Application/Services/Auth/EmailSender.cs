using Application.Interfaces.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;


namespace Application.Services.Auth
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env; 

        public EmailSender(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var host = _config["EMAIL_SMTP_HOST"] ?? throw new InvalidOperationException("EMAIL_SMTP_HOST is not configured");
            var port = _config.GetValue<int>("EMAIL_SMTP_PORT", 587);
            if (port <= 0)
                throw new ArgumentOutOfRangeException(nameof(port), "EMAIL_SMTP_PORT must be a positive, non-zero value");

            var username = _config["EMAIL_SMTP_USERNAME"] ?? throw new InvalidOperationException("EMAIL_SMTP_USERNAME is not configured");
            var password = _config["EMAIL_SMTP_PASSWORD"] ?? throw new InvalidOperationException("EMAIL_SMTP_PASSWORD is not configured");
            var from = _config["EMAIL_FROM"] ?? throw new InvalidOperationException("EMAIL_FROM is not configured");

            // Load the HTML template
            var templatePath = Path.Combine(_env.WebRootPath, "email-templates", "confirm-email.html");
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Email template not found", templatePath);

            var htmlTemplate = await File.ReadAllTextAsync(templatePath);
            var finalHtml = htmlTemplate.Replace("{CONFIRMATION_LINK}", htmlMessage); 

            var smtpClient = new SmtpClient(host)
            {
                Port = port,
                Credentials = new System.Net.NetworkCredential(username, password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = finalHtml,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
