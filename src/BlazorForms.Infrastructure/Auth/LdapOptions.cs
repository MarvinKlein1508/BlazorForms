namespace BlazorForms.Infrastructure.Auth;

/// <summary>
/// Options used to configure LDAP integration for authentication and directory lookups.
/// </summary>
public sealed class LdapOptions
{
    /// <summary>
    /// Configuration section name used to bind <see cref="LdapOptions"/> from configuration providers.
    /// </summary>
    public const string SectionName = "LdapOptions";

    /// <summary>
    /// Enables or disables LDAP authentication integration.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether user accounts are created automatically when needed.
    /// </summary>
    /// <remarks>When this property is set to <see langword="true"/>, the system will automatically create
    /// user accounts for new users as required. If set to <see langword="false"/>, automatic user account creation is
    /// disabled and new users must be added manually.</remarks>
    public bool ShouldCreateUsers { get; set; }

    /// <summary>
    /// Hostname or address of the LDAP server.
    /// Examples: "ldap.example.com", "ldap://ldap.example.com:389".
    /// </summary>
    public string LDAP_SERVER { get; set; } = string.Empty;

    /// <summary>
    /// NetBIOS or Active Directory domain name used for authentication and lookups.
    /// Example: "EXAMPLE" or "example.com".
    /// </summary>
    public string DOMAIN_SERVER { get; set; } = string.Empty;

    /// <summary>
    /// Distinguished Name (DN) used to bind or search the directory.
    /// Example: "CN=ServiceAccount,OU=Users,DC=example,DC=com".
    /// </summary>
    public string DistinguishedName { get; set; } = string.Empty;

    /// <summary>
    /// Base Organizational Unit (OU) distinguished name where group searches are performed.
    /// Example: "OU=Groups,DC=example,DC=com".
    /// </summary>
    public string GroupBaseOU { get; set; } = string.Empty;
}
