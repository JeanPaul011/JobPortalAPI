using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace JobPortalAPI.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;

            // ✅ Ensure `.env` variables override `appsettings.json`
            _emailSettings.SmtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? _emailSettings.SmtpServer;
            _emailSettings.SmtpPort = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out int port) ? port : _emailSettings.SmtpPort;
            _emailSettings.SenderEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL") ?? _emailSettings.SenderEmail;
            _emailSettings.SenderPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? _emailSettings.SenderPassword;
            _emailSettings.SenderName = Environment.GetEnvironmentVariable("SMTP_SENDER_NAME") ?? _emailSettings.SenderName;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress("Recipient", toEmail));
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = body };

                using (var client = new SmtpClient())
                {
                    // ✅ Print Debugging Information
                    Console.WriteLine("🚀 DEBUG: Checking EmailService Configuration...");
                    Console.WriteLine($"✅ SMTP_SERVER: {_emailSettings.SmtpServer}");
                    Console.WriteLine($"✅ SMTP_PORT: {_emailSettings.SmtpPort}");
                    Console.WriteLine($"✅ SMTP_EMAIL: {_emailSettings.SenderEmail}");
                    Console.WriteLine($"✅ SMTP_PASSWORD: {_emailSettings.SenderPassword?.Substring(0, 3)}******"); // Masked

                    await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                    Console.WriteLine("✅ Email sent successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Email sending failed: {ex.Message}");
                throw;
            }
        }
    }
}
