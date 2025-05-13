using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Application.Domain;
public class Role : IDbModel<int?>, IDbParameterizable
{
    public int RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ActiveDirectoryGroupCN { get; set; } = string.Empty;
    public bool CanBeDeleted { get; set; }
    public List<RolePermission> Permissions { get; set; } = [];
    public int? GetIdentifier() => RoleId > 0 ? RoleId : null;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "ROLE_ID", RoleId },
            { "NAME", Name },
            { "ACTIVE_DIRECTORY_GROUP_CN", ActiveDirectoryGroupCN },
            { "CAN_BE_DELETED", CanBeDeleted },
        };
    }
}
