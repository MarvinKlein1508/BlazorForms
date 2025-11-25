namespace BlazorForms.Infrastructure.Auth;

public class LdapResult
{
    public Guid? Guid { get; set; }
    public List<string> Groups { get; set; } = [];
    public Dictionary<string, string> Attributes { get; set; } = [];
}

