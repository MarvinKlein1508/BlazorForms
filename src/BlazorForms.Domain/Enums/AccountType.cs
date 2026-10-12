namespace BlazorForms.Domain;

/// <summary>
/// Type of the user account indicating how authentication is performed.
/// </summary>
public enum AccountType
{
    /// <summary>
    /// Internal application-managed account (username/password stored in the app).
    /// </summary>
    Internal = 1,

    /// <summary>
    /// Account authenticated via Active Directory / external identity provider.
    /// </summary>
    ActiveDirectory = 2
}
