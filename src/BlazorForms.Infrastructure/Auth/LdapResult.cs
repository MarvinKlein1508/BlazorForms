namespace BlazorForms.Infrastructure.Auth;

/// <summary>
/// Represents the result returned from an LDAP lookup or authentication attempt.
/// Contains the resolved object identifier (when present), the group's membership list,
/// and any additional attribute key/value pairs retrieved from LDAP.
/// </summary>
public class LdapResult
{
    /// <summary>
    /// The LDAP object's unique identifier, if available.
    /// </summary>
    public Guid? Guid { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user should be created if it does not exist.
    /// </summary>
    public bool ShouldCreate { get; set; }

    /// <summary>
    /// Names or distinguished names of groups the LDAP object is a member of.
    /// </summary>
    public List<string> Groups { get; set; } = [];

    /// <summary>
    /// Additional LDAP attributes retrieved for the object.
    /// Keys are attribute names and values are the attribute values as strings.
    /// </summary>
    public Dictionary<string, string> Attributes { get; set; } = [];
}
