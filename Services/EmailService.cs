using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace JobPortalAPI.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;

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

                using var client = new SmtpClient();

                client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                {
                    if (sslPolicyErrors == SslPolicyErrors.None)
                        return true;

                    Console.WriteLine($"SSL Policy Errors: {sslPolicyErrors}");

                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    if (string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Development mode: Ignoring SSL certificate errors.");
                        return true;
                    }

                    Console.WriteLine("Production mode: Rejecting untrusted SSL certificate.");
                    return false;
                };

                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                throw;
            }
        }
        public async Task SendVerificationEmail(string email, string verificationLink)
        {
            var subject = "Verify Your Email Address";
            var body = $@"
        <h2>Welcome to JobPortal!</h2>
        <p>Please verify your email address by clicking the link below:</p>
        <a href='{verificationLink}'>{verificationLink}</a>
        <p>This link will expire in 24 hours.</p>
    ";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}