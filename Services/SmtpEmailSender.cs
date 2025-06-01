using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using MailKit.Security;

namespace BlazorOfficina.Services    // ← deve essere lo stesso namespace di IEmailSender!
{
    // ← assicurati di avere QUI “: IEmailSender”
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtp;

        public SmtpEmailSender(IOptions<SmtpSettings> opts)
        {
            _smtp = opts.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(_smtp.From));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;
            msg.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();

            // Usa Auto per lasciare che MailKit scelga la modalità giusta
            await client.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.Auto);

            await client.AuthenticateAsync(_smtp.Username, _smtp.Password);
            await client.SendAsync(msg);
            await client.DisconnectAsync(true);
        }

    }
}
