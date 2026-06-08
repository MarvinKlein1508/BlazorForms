using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

namespace BlazorForms.Core.Infrastructure;
public sealed class GraphSender : IEmailSender
{
    private readonly GraphOptions _options;

    public MailProvider Provider => MailProvider.MicrosoftGraph;

    public GraphSender(IOptions<GraphOptions> options)
    {
        _options = options.Value;
    }
    public async Task SendAsync(EmailMessage message)
    {
        var graphMessage = ConvertToGraphMessage(message);
        
        if (graphMessage.ToRecipients?.Count == 0 && graphMessage.CcRecipients?.Count == 0 && graphMessage.BccRecipients?.Count == 0)
        {
            return;
        }

        // NOT IN PRODUCTION
        if (_options.Debug)
        {
            DebugMail(graphMessage);
        }

        var credential = new ClientSecretCredential(_options.TenantId, _options.ClientId, _options.ClientSecret);
        using var graphClient = new GraphServiceClient(credential, ["https://graph.microsoft.com/.default"]);
       
        var requestBody = new SendMailPostRequestBody
        {
            Message = graphMessage,
            SaveToSentItems = true
        };
        
        await graphClient.Users[_options.SenderEmail].SendMail.PostAsync(requestBody);
    }
    private static void DebugMail(Message message)
    {
        static string FormatRecipient(Recipient recipient)
        {
            var address = recipient.EmailAddress?.Address ?? string.Empty;
            var name = recipient.EmailAddress?.Name;

            return string.IsNullOrWhiteSpace(name)
                ? address
                : $"{name} <{address}>";
        }

        var newline = Environment.NewLine;

        var recipientsTo = message.ToRecipients?
            .Select(FormatRecipient)
            .ToList() ?? [];

        var recipientsCc = message.CcRecipients?
            .Select(FormatRecipient)
            .ToList() ?? [];

        var recipientsBcc = message.BccRecipients?
            .Select(FormatRecipient)
            .ToList() ?? [];

        var recipientsInfoPlain =
            "Ursprüngliche Empfänger:" + newline +
            "To:" + newline + (recipientsTo.Any() ? string.Join(newline, recipientsTo) : "Keine") + newline +
            "Cc:" + newline + (recipientsCc.Any() ? string.Join(newline, recipientsCc) : "Keine") + newline +
            "Bcc:" + newline + (recipientsBcc.Any() ? string.Join(newline, recipientsBcc) : "Keine") + newline + newline;

        var recipientsInfoHtml =
            "<div style=\"border:1px solid #ccc; padding:10px; margin-bottom:10px;\">" +
            "<strong>Ursprüngliche Empfänger:</strong><br/>" +
            "To:<br/>" + (recipientsTo.Any() ? string.Join("<br/>", recipientsTo) : "Keine") + "<br/>" +
            "Cc:<br/>" + (recipientsCc.Any() ? string.Join("<br/>", recipientsCc) : "Keine") + "<br/>" +
            "Bcc:<br/>" + (recipientsBcc.Any() ? string.Join("<br/>", recipientsBcc) : "Keine") +
            "</div>";

        message.Body ??= new ItemBody
        {
            ContentType = BodyType.Text,
            Content = string.Empty
        };

        if (message.Body.ContentType == BodyType.Html)
        {
            message.Body.Content = recipientsInfoHtml + (message.Body.Content ?? string.Empty);
        }
        else
        {
            message.Body.Content = recipientsInfoPlain + (message.Body.Content ?? string.Empty);
        }

        message.ToRecipients =
        [
            new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Name = "Marvin Klein",
                    Address = "marvin.klein@harold-scholz.de"
                }
            }
        ];

        message.CcRecipients = [];
        message.BccRecipients = [];
    }

    private static Message ConvertToGraphMessage(EmailMessage emailMessage)
    {
        var graphMessage = new Message
        {
            Subject = emailMessage.Subject,
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = emailMessage.Body
            },
            ToRecipients = emailMessage.To?.Select(emailAddress => new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Name = emailAddress,
                    Address = emailAddress
                }
            }).ToList(),
            CcRecipients = emailMessage.Cc?.Select(emailAddress => new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Name = emailAddress,
                    Address = emailAddress
                }
            }).ToList(),
            BccRecipients = emailMessage.Bcc?.Select(emailAddress => new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Name = emailAddress,
                    Address = emailAddress
                }
            }).ToList(),
            Attachments = emailMessage.Attachments?.Select(attachment =>
            {
                var (filename, content) = attachment;
                return new FileAttachment
                {
                    Name = $"{filename}.pdf",
                    ContentBytes = content,
                    ContentType = "application/pdf",
                    OdataType = "#microsoft.graph.fileAttachment"
                } as Attachment;
            }).ToList()
        };

        return graphMessage;
    }
}
