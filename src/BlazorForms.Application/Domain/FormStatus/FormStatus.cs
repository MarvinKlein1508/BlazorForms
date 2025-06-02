using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Application.Domain;
public class FormStatus : LocalizationModelBase<FormStatusDescription>, IDbModel<int?>
{
    public int StatusId { get; set; }
    public bool RequiresApproval { get; set; }
    public bool IsCompleted { get; set; }
    public int SortOrder { get; set; }
    public int? GetIdentifier() => StatusId > 0 ? StatusId : null;
}
