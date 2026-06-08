using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BlazorForms.Core.Infrastructure;

public sealed class SmtpSender : IEmailSender
{
    private readonly SmtpOptions _options;
    public MailProvider Provider => MailProvider.Smtp;
    public SmtpSender(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }
    public Task SendAsync(EmailMessage message)
    {
        var mimeMessage = ConvertToMimeMessage(message);
        mimeMessage.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));

        using var client = new SmtpClient();
        if (_options.UseSSL)
        {
            client.Connect(_options.Host, _options.Port, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
        }
        else
        {
            client.Connect(_options.Host, _options.Port, false);
        }

        // Note: only needed if the SMTP server requires authentication
        if (!string.IsNullOrWhiteSpace(_options.Username) && !string.IsNullOrWhiteSpace(_options.Password))
        {
            client.Authenticate(_options.Username, _options.Password);
        }

        var response = client.Send(mimeMessage);
        client.Disconnect(true);

        return Task.CompletedTask;
    }

    private static MimeMessage ConvertToMimeMessage(EmailMessage message)
    {
        MimeMessage mimeMessage = new()
        {
            Subject = message.Subject
        };

        foreach (var emailAdress in message.To)
        {
            mimeMessage.To.Add(new MailboxAddress(emailAdress, emailAdress));
        }

        foreach (var emailAdress in message.Cc)
        {
            mimeMessage.Cc.Add(new MailboxAddress(emailAdress, emailAdress));
        }

        foreach (var emailAdress in message.Bcc)
        {
            mimeMessage.Bcc.Add(new MailboxAddress(emailAdress, emailAdress));
        }

        var body = new TextPart("html")
        {
            Text = message.Body,
        };

        var multipart = new Multipart("mixed")
        {
            body
        };

        foreach ((string filename, byte[] content) in message.Attachments)
        {
            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(new MemoryStream(content)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = $"{filename}.pdf"
            };

            multipart.Add(attachment);
        }

        mimeMessage.Body = multipart;

        return mimeMessage;
    }
}
