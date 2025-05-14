namespace BlazorForms.Application.Domain;

public class User : IDbModel<int?>, IDbParameterizable
{
    public int UserId { get; set; }
    public UserGroup UserGroupId { get; set; } = UserGroup.Users;
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Guid? ActiveDirectoryGuid { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;

    public int? GetIdentifier() => UserId > 0 ? UserId : null;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "USER_ID", UserId },
            { "USER_GROUP_ID", (int)UserGroupId },
            { "USERNAME", Username },
            { "DISPLAY_NAME", DisplayName },
            { "ACTIVE_DIRECTORY_GUID", ActiveDirectoryGuid },
            { "EMAIL", Email },
            { "PASSWORD", Password },
            { "SALT", Salt },
            { "ORIGIN", Origin },
        };
    }
}
