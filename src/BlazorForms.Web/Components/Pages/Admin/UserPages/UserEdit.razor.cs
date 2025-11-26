using BlazorForms.Domain.Entities;
using BlazorForms.Infrastructure.Repositories;
using BlazorForms.Web.Components.ComponentBases;

namespace BlazorForms.Web.Components.Pages.Admin.UserPages;

public partial class UserEdit : EditPageBase<User, UserRepository, int?>
{
    protected override string GetListUrl() => "/Admin/Users";

    public bool ActiveDirectoryMode => Input?.Origin is "ad";
}
