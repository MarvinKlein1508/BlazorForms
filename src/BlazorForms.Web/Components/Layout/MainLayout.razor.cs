using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorForms.Web.Components.Layout;

public partial class MainLayout
{
    [Inject]
    public required IJSRuntime JSRuntime { get; set; }
    private async Task SwitchThemeAsync()
    {
        await JSRuntime.InvokeVoidAsync("Blazor.theme.switchTheme");
    }
}
