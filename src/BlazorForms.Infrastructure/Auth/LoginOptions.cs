namespace BlazorForms.Infrastructure.Auth;

public class LoginOptions
{
    public const string SectionName = "LoginOptions";
    public bool IsLocalLoginEnabled { get; set; }
    public bool IsLdapLoginEnabled { get; set; }
    public LdapSettings LdapSettings { get; set; } = new();
}
