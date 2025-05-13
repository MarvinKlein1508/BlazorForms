
namespace BlazorForms.Application.Domain;

public class User : IDbParameterizable
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Guid? ActiveDirectoryGuid { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;

    public List<UserRole> Roles { get; set; } = [];

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "USER_ID", UserId },
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
