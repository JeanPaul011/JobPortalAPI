using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace JobPortalAPI.Services
{
    public class EmailService
    {
        public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                // Fix: Use Correct Environment Variable Names
                string smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? throw new Exception("SMTP_SERVER is missing!");
                int smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
                string senderEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL") ?? throw new Exception("SMTP_EMAIL is missing!");
                string senderPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new Exception("SMTP_PASSWORD is missing!");
                string senderName = Environment.GetEnvironmentVariable("SMTP_SENDER_NAME") ?? "Job Portal";

                //  Debugging Output
                Console.WriteLine(" Email Configuration:");
                Console.WriteLine($"SMTP Server: {smtpServer}");
                Console.WriteLine($"SMTP Port: {smtpPort}");
                Console.WriteLine($"Sender Email: {senderEmail}");
                Console.WriteLine($"Sender Name: {senderName}");
                Console.WriteLine($"Sending Email To: {recipientEmail}");

                // Setup SMTP Client
                using var client = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                // Create Email Message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);
                
                //  Send Email
                await client.SendMailAsync(mailMessage);
                Console.WriteLine($" Email successfully sent to {recipientEmail}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Email Sending Failed: {ex.Message}");
                return false;
            }
        }
    }
}
