using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BlazorForms.Core.Infrastructure;

public class EmailService
{
    private readonly IEmailSenderFactory _factory;
    private readonly AppSettings _settings;
    

    public EmailService(IEmailSenderFactory factory, IOptions<AppSettings> settings)
    {
        _factory = factory;
        _settings = settings.Value;
    }
    public async Task SendAsync(EmailMessage message)
    {
        var sender = _factory.CreateSender(_settings.MailProvider);
        await sender.SendAsync(message);
    }
}
