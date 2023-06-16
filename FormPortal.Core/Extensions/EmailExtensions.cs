using FormPortal.Core.Models;
using FormPortal.Core.Pdf;
using FormPortal.Core.Services;
using FormPortal.Core.Settings;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Components;
using MimeKit;
using System.Globalization;
using System.Runtime.CompilerServices;
using static iText.IO.Util.IntHashtable;

namespace FormPortal.Core.Extensions
{
    public static class EmailExtensions
    {
        public static void SendMail(MimeMessage email, EmailSettings settings)
        {
            using var client = new SmtpClient();
            if (settings.UseSSL)
            {
                client.Connect(settings.Host, settings.Port, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
            }
            else
            {
                client.Connect(settings.Host, settings.Port, false);
            }

            // Note: only needed if the SMTP server requires authentication
            if (!string.IsNullOrWhiteSpace(settings.Username) && !string.IsNullOrWhiteSpace(settings.Password))
            {
                client.Authenticate(settings.Username, settings.Password);
            }

            var response = client.Send(email);
            client.Disconnect(true);
        }

        public static Task SendMailForEntryStatusChangeAsync(this FormEntryStatusChange status_change, List<string> emailAdresses, FormEntry entry, string baseUrl, EmailSettings settings)
        {
            MimeMessage email = new MimeMessage();
            email.From.Add(new MailboxAddress(settings.SenderName, settings.SenderEmail));
            foreach (var emailAdress in emailAdresses)
            {
                if (StringExtensions.IsEmail(emailAdress))
                {
                    email.To.Add(new MailboxAddress(emailAdress, emailAdress));
                }
            }

            if (email.To.Any())
            {
                email.Subject = $"Statusänderung des Formulareintrags {entry.Name} ({entry.Id})";

                var status = AppdatenService.Get<FormStatus>(status_change.StatusId);
                var status_description = status?.GetLocalization(CultureInfo.CurrentCulture) ?? new();
                var body = new TextPart("html")
                {
                    Text =
$"""
Der Status des Formulareintrages {entry.Name} für das Formular {entry.Form.Name} wurde geändert. 
<br /><br />
Neuer Status: {status_description.Name}
Kommentar: {status_change.Comment}
<br /><br />
<a href="{baseUrl}Entry/{entry.EntryId}">Klicken Sie hier</a> um den Formulareintrag einzusehen
"""
                };

                // now create the multipart/mixed container to hold the message text and the
                // image attachment
                var multipart = new Multipart("mixed")
                {
                    body
                };

                // now set the multipart/mixed as the message body
                email.Body = multipart;


                SendMail(email, settings);
            }

            return Task.CompletedTask;
        }
    }
}
