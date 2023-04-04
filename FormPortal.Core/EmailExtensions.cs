using FormPortal.Core.Settings;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormPortal.Core
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
    }
}
