using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public void SendEmail(string toEmail, string subject, string body)
    {
        var settings = _config.GetSection("EmailSettings");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(settings["SenderName"], settings["SenderEmail"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using (var client = new SmtpClient())
        {
            client.Connect(settings["SmtpServer"], int.Parse(settings["Port"]), false);
            client.Authenticate(settings["SenderEmail"], settings["SenderPassword"]);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}
