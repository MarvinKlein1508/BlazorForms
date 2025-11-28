using BlazorForms.Domain.Entities;
using BlazorForms.Infrastructure.Repositories;
using BlazorForms.Web.Components.ComponentBases;

namespace BlazorForms.Web.Components.Pages.Admin.FormPages;

public partial class FormCreate : CreatePageBase<Form, FormRepository, int?>
{
    protected override string GetEntityRedirectUrl()
    {
        throw new NotImplementedException();
    }
}
