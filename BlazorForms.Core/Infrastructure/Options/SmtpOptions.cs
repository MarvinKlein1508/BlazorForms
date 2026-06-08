namespace BlazorForms.Core.Infrastructure;

public class SmtpOptions
{
    public const string SectionName = "SmtpOptions";
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 25;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseSSL { get; set; } = true;
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string ReplyTo { get; set; } = string.Empty;
}