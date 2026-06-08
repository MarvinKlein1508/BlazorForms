namespace BlazorForms.Core.Infrastructure;

public interface IEmailSender
{
    MailProvider Provider { get; }

    Task SendAsync(EmailMessage message);
}
