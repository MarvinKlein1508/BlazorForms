namespace BlazorForms.Infrastructure.Auth;

public class LdapSettings
{
    public string LdapServer { get; set; } = string.Empty;
    public string DomainServer { get; set; } = string.Empty;
    public string DistinguishedName { get; set; } = string.Empty;
    public string GroupBaseOU { get; set; } = string.Empty;
}
