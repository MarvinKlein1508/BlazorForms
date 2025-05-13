namespace BlazorForms.Application.Domain;

public class User
{
    public int UserId { get; set; }
    public int? UserGroupId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Guid? ActiveDirectoryGuid { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
}
