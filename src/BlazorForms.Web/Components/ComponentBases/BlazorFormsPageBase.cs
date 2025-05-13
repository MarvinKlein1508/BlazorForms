using BlazorForms.Application.Database;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Web.Components.ComponentBases;

public abstract class BlazorFormsPageBase : ComponentBase
{
    [Inject]
    public IDbConnectionFactory DbFactory { get; set; } = default!;
}
