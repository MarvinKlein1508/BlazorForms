namespace BlazorForms.Core.Infrastructure;

public class NoneSender : IEmailSender
{
    public MailProvider Provider => MailProvider.None;

    public Task SendAsync(EmailMessage message)
    {
        return Task.CompletedTask;
    }
}
