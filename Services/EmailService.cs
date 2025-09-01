using System.Net;
using System.Net.Mail;

namespace Student_Portal.Services;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["Gmail:Username"]),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);

        using var client = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(
                _configuration["Gmail:Username"],
                _configuration["Gmail:Password"]
            ),
            EnableSsl = true
        };

        await client.SendMailAsync(mailMessage);
    }
}