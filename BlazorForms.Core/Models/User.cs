namespace BlazorForms.Core.Models;

public sealed class User : IDbModel<int?>
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Guid? ActiveDirectoryGuid { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;

    public List<Permission> Permissions { get; set; } = new();
    /// <summary>
    /// Gets or sets a flag for a form manager to receive an email for new form entries
    /// </summary>
    public bool EmailEnabled { get; set; }
    /// <summary>
    /// Used to identify form managers which are able to approve forms with approval <see cref="FormStatus"/>
    /// </summary>
    public bool CanApprove { get; set; }
    /// <summary>
    /// Used to identify form managers which getting pre-selected to receive notifications on status changes
    /// </summary>
    public bool StatusChangeNotificationDefault { get; set; }

    public int? GetIdentifier()
    {
        return UserId > 0 ? UserId : null;
    }
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "USER_ID", UserId },
            { "USERNAME", Username },
            { "DISPLAY_NAME", DisplayName },
            { "ACTIVE_DIRECTORY_GUID", ActiveDirectoryGuid?.ToString() },
            { "EMAIL", Email },
            { "PASSWORD", Password },
            { "SALT", Salt },
            { "ORIGIN", Origin },

        };
    }
}
