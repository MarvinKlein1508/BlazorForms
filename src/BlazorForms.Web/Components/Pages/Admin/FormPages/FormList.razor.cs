using BlazorForms.Domain.Entities;
using BlazorForms.Domain.Filters;
using BlazorForms.Infrastructure.Repositories;
using BlazorForms.Web.Components.ComponentBases;

namespace BlazorForms.Web.Components.Pages.Admin.FormPages;

public partial class FormList : ListPageBase<Form, FormRepository, FormFilter>
{

}
