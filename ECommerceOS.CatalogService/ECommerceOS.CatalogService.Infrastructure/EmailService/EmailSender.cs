using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace ECommerceOS.CatalogService.Infrastructure.EmailService;

public class EmailSender(IOptions<SmtpClientOptions> options) : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var credentials = options.Value;

        var client = new SmtpClient(credentials.Host, credentials.Port)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(credentials.EmailAddress, credentials.Password)
        };

        var message = new MailMessage(from: credentials.EmailAddress, to: email, subject: subject, body: htmlMessage);
        message.IsBodyHtml = true;

        return client.SendMailAsync(message);
    }
}
    