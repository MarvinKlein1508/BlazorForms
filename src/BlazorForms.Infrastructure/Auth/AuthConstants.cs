namespace BlazorForms.Infrastructure.Auth;

/// <summary>
/// Contains authentication-related constants used across the application.
/// </summary>
public static class AuthConstants
{
    /// <summary>
    /// Name of the cookie used to record that a user has a pending two-factor authentication (2FA).
    /// </summary>
    public const string TwoFactorCookieName = "2FA";
}
