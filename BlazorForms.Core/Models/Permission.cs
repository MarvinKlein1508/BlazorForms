using BlazorForms.Core.Interfaces;

namespace BlazorForms.Core.Models;

public class Permission : LocalizationModelBase<PermissionDescription>
{
    public int PermissionId { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public int Id => PermissionId;
    public IEnumerable<Dictionary<string, object?>> GetLocalizedParameters()
    {
        foreach (var description in Description)
        {
            yield return new Dictionary<string, object?>
            {
                { "PERMISSION_ID", PermissionId },
                { "NAME", description.Name },
                { "DESCRIPTION", description.Description }
            };
        }
    }

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "PERMISSION_ID", PermissionId },
            { "IDENTIFIER", Identifier }
        };
    }
}

public class PermissionDescription : ILocalizationHelper
{
    public int PermissionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
