namespace AppliFilms.Api.Services;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_config["EmailSettings:From"]));
        message.To.Add(MailboxAddress.Parse(_config["EmailSettings:To"]));
        message.Subject = subject;

        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        // Gmail impose SSL/TLS
        await client.ConnectAsync(
            _config["EmailSettings:SmtpServer"],
            int.Parse(_config["EmailSettings:Port"]),
            SecureSocketOptions.StartTls
        );

        await client.AuthenticateAsync(
            _config["EmailSettings:Username"],
            _config["EmailSettings:Password"]
        );

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}