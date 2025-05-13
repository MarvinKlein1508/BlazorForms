using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Application.Domain;
public class Permission : IDbModel<int?>
{
    public int PermissionId { get; set; }
    public string Identifier { get; set; } = string.Empty;

    public int? GetIdentifier() => PermissionId > 0 ? PermissionId : null;
}
