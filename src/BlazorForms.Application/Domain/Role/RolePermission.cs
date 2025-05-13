namespace BlazorForms.Application.Domain;

public class RolePermission : IDbParameterizable
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "ROLE_ID", RoleId },
            { "PERMISSION_ID", PermissionId },
            { "IS_ACTIVE", IsActive }
        };
    }
}
