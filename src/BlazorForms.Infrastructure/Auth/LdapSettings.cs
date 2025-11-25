namespace BlazorForms.Infrastructure.Auth;

public class LdapSettings
{
    public string LdapServer { get; set; } = string.Empty;
    public string DomainServer { get; set; } = string.Empty;
    public string DistinguishedName { get; set; } = string.Empty;
    public string AdminGroupDistinguishedName { get; set; } = string.Empty;
    public string EditorGroupDistinguishedName { get; set; } = string.Empty;
}
