namespace BlazorForms.Core.Infrastructure;

public interface IEmailSenderFactory
{
    IEmailSender CreateSender(MailProvider provider);
}
