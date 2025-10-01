
using BlazorForms.Application.Domain;

namespace BlazorForms.Web.Components.Pages.Admin;

public partial class ConfigPage
{
    public Config? Input { get; set; }
    protected override async Task OnInitializedAsync()
    {
        using var connection = await DbFactory.CreateConnectionAsync();
        Input = await ConfigRepository.GetMainConfig(connection);
    }
}
