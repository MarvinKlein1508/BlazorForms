namespace BlazorForms.Domain;

public class User : IDbModel<int?>, IDbParameterizable
{
    /// <summary>
    /// Primary key identifier for the user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Unique username used for authentication.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's role. Defaults to <see cref="UserRole.User"/>.
    /// </summary>
    public UserRole Role { get; set; } = UserRole.User;

    /// <summary>
    /// Human-friendly display name for the user.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Email address for the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Optional GUID from Active Directory when the account is backed by AD.
    /// </summary>
    public Guid? ActiveDirectoryGuid { get; set; }

    /// <summary>
    /// Hashed password for internal accounts.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Salt used when hashing the password.
    /// </summary>
    public string Salt { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the account is an internal application account or an Active Directory account.
    /// </summary>
    public AccountType AccountType { get; set; } = AccountType.Internal;

    /// <summary>
    /// True when the account is active; false when disabled.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// True when two-factor authentication is enabled for the account.
    /// </summary>
    public bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// If set, the time until which the user is locked out from signing in.
    /// </summary>
    public DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// True when lockout is enabled for the user (failed attempts can lock the account).
    /// </summary>
    public bool LockoutEnabled { get; set; }

    /// <summary>
    /// Number of consecutive failed access attempts.
    /// </summary>
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// Token used for two-factor authentication flows (if applicable).
    /// </summary>
    public string? TwoFactorToken { get; set; }

    public int? GetIdentifier() => UserId > 0 ? UserId : null;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "USER_ID", UserId },
            { "USERNAME", Username },
            { "ROLE", (int)Role },
            { "DISPLAY_NAME", DisplayName },
            { "EMAIL", Email },
            { "ACTIVE_DIRECTORY_GUID", ActiveDirectoryGuid },
            { "PASSWORD", Password },
            { "SALT", Salt },
            { "ACCOUNT_TYPE", (int)AccountType },
            { "IS_ACTIVE", IsActive },
            { "TWO_FACTOR_ENABLED", TwoFactorEnabled },
            { "LOCKOUT_END", LockoutEnd },
            { "LOCKOUT_ENABLED", LockoutEnabled },
            { "ACCESS_FAILED_COUNT", AccessFailedCount },
            { "TWO_FACTOR_TOKEN", TwoFactorToken }
        };
    }
}
