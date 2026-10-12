using BlazorForms.Domain;
using BlazorForms.Infrastructure.Database;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorForms.Components;

public abstract class BlazorFormsPageBase : ComponentBase
{
    //[Inject] protected AuthService AuthService { get; set; } = default!;
    [Inject] protected IDbConnectionFactory DbFactory { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] protected ProtectedLocalStorage LocalStorage { get; set; } = default!;
    [Inject] protected INotificationService NotificationService { get; set; } = default!;
    [Inject] protected IDialogService DialogService { get; set; } = default!;
    
    protected User? CurrentUser { get; set; }

    protected override async Task OnInitializedAsync()
    {
        //CurrentUser = await AuthService.GetUserAsync();
        //Log.Information("Visit: {url}; UserId: {user}", NavigationManager.Uri, CurrentUser?.DisplayName.ToString() ?? "<UNBEKANNT>");
    }
}
