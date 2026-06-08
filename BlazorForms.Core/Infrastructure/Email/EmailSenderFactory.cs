namespace BlazorForms.Core.Infrastructure;

public sealed class EmailSenderFactory : IEmailSenderFactory
{
    private readonly Dictionary<MailProvider, IEmailSender> _senders;

    public EmailSenderFactory(IEnumerable<IEmailSender> senders)
    {
        _senders = senders.ToDictionary(s => s.Provider, s => s);
    }
    public IEmailSender CreateSender(MailProvider provider)
    {
        if (!_senders.TryGetValue(provider, out var sender))
        {
            throw new ArgumentOutOfRangeException(nameof(provider), $"No email sender registered for provider {provider}");
        }

        return sender;
    }
}