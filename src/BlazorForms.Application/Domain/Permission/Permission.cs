using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Application.Domain;
public class Permission
{
    public int PermissionId { get; set; }
    public string Identifier { get; set; } = string.Empty;
}
