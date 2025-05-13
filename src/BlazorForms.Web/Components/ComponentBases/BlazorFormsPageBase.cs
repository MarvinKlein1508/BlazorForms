using BlazorForms.Application.Database;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace BlazorForms.Web.Components.ComponentBases;

public abstract class BlazorFormsPageBase : ComponentBase
{
    [Inject] protected IDbConnectionFactory DbFactory { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] protected ProtectedLocalStorage LocalStorage { get; set; } = default!;
}
