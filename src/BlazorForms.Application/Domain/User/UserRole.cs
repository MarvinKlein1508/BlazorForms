namespace BlazorForms.Application.Domain;

public class UserRole : IDbParameterizable
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public bool IsActive { get; set; }

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "USER_ID", UserId },
            { "ROLE_ID", RoleId },
            { "IS_ACTIVE", IsActive }
        };
    }
}
