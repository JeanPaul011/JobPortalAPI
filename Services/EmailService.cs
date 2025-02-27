using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace JobPortalAPI.Services
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
    }

    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer)
                {
                    Port = _emailSettings.SmtpPort,
                    Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);
                await client.SendMailAsync(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Email Sending Failed: {ex.Message}");
                return false;
            }
        }
    }
}

