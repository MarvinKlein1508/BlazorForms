using BlazorForms.Domain.Entities;
using BlazorForms.Infrastructure;

namespace BlazorForms.Web.Components.Pages.Admin.FormStatusPages;

public partial class FormStatusCreate
{
    protected override string GetEntityRedirectUrl() => $"/Admin/Status/Edit?Id={Input!.GetIdentifier()}";

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        var descriptions = Storage.Get<Language>().Select(l => new FormStatusDescription { Code = l.Code });
        Input!.Descriptions.AddRange(descriptions);
    }
}
