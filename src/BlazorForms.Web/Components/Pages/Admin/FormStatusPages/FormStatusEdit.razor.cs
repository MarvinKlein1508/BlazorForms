using BlazorForms.Domain.Entities;
using BlazorForms.Infrastructure.Repositories;
using BlazorForms.Web.Components.ComponentBases;

namespace BlazorForms.Web.Components.Pages.Admin.FormStatusPages;

public partial class FormStatusEdit : EditPageBase<FormStatus, FormStatusRepository, int?>
{
    protected override string GetListUrl() => "/Admin/Status";
}
