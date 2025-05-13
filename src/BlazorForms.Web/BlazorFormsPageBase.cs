using BlazorForms.Application.Database;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Web;

public class BlazorFormsPageBase : ComponentBase
{
    [Inject]
    public IDbConnectionFactory DbFactory { get; set; } = default!;
}
