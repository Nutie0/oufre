using System.Net;
using System.Net.Mail;

namespace registerAPI.Services;

public class EmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromEmail;

    public EmailService(IConfiguration config)
    {
        _fromEmail = config["Email:Address"];

        _smtpClient = new SmtpClient(config["Email:Host"])
        {
            Port = int.Parse(config["Email:Port"]),
            Credentials = new NetworkCredential(_fromEmail, config["Email:Password"]),
            EnableSsl = true
        };
    }

    public async Task SendVerificationEmail(string toEmail, string token, string pin)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_fromEmail, "Verification Service"),
            Subject = "Verify Your Email",
            Body = $@"<h1>Vérification d'email</h1>
            <p>Votre code PIN : <strong>{pin}</strong></p>
          <p>Cliquez sur ce lien pour activer votre compte :</p>
          <a href='https://localhost:5157/api/auth/verify?token={token}&pin='>Vérifier mon email</a>",
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);
        await _smtpClient.SendMailAsync(mailMessage);
    }
}