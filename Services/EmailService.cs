using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace LeaveManagementSystem.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration? config = null)
        {
            // Handle null configuration gracefully
            _config = config ?? new ConfigurationBuilder().Build();
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var settings = _config.GetSection("EmailSettings");

                var message = new MimeMessage();
                var senderEmail = settings["SenderEmail"] ?? "noreply@example.com";
                var senderName = settings["SenderName"] ?? "Leave Management System";
                
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                using (var client = new SmtpClient())
                {
                    var smtpServer = settings["SmtpServer"] ?? "smtp.gmail.com";
                    var portString = settings["Port"] ?? "587";
                    var port = int.TryParse(portString, out int parsedPort) ? parsedPort : 587;
                    var senderPassword = settings["SenderPassword"] ?? "";
                    
                    client.Connect(smtpServer, port, false);
                    client.Authenticate(senderEmail, senderPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                // Log or handle email sending errors gracefully
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
    }
}