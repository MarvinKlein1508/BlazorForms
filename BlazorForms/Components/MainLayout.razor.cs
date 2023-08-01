using BlazorBootstrap;
using BlazorForms.Core.Constants;
using BlazorForms.Core.Services;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace BlazorForms.Components
{
    public partial class MainLayout
    {
        private Sidebar _sidebar = default !;
        IEnumerable<NavItem> navItems = default!;
        private async Task<SidebarDataProviderResult> SidebarDataProvider(SidebarDataProviderRequest request)
        {
            if (navItems is null)
                navItems = await GetNavItems().ToListAsync();
            return await Task.FromResult(request.ApplyTo(navItems));
        }

        private async IAsyncEnumerable<NavItem> GetNavItems()
        {
            yield return new NavItem
            {
                Id = "1",
                Href = "/",
                IconName = IconName.HouseDoorFill,
                Text = "Dashboard"
            };

            var checkRoles = await authService.CheckRoles(Roles.EDIT_FORMS, Roles.EDIT_ENTRIES, Roles.EDIT_USERS, Roles.EDIT_STATUS);
            

            if (checkRoles.Any(x => x.Value))
            {
                yield return new NavItem
                {
                    Id = "7",
                    IconName = IconName.Gear,
                    Text = "Administration",
                    IconColor = IconColor.Success
                };
            }

            if (checkRoles[Roles.EDIT_USERS])
            {
                yield return new NavItem
                {
                    Id = "8",
                    Href = "/Admin/Users",
                    IconName = IconName.PeopleFill,
                    Text = "Benutzerverwaltung",
                    ParentId = "7"
                };
            }

            if (checkRoles[Roles.EDIT_STATUS])
            {
                yield return new NavItem
                {
                    Id = "11",
                    Href = "/Admin/Status",
                    IconName = IconName.ListCheck,
                    Text = "Statusverwaltung",
                    ParentId = "7"
                };
            }

            if (checkRoles[Roles.EDIT_FORMS])
            {
                yield return new NavItem
                {
                    Id = "10",
                    Href = "/Admin/Forms",
                    IconName = IconName.Files,
                    Text = "Formulare",
                    ParentId = "7"
                };
            }

            if (checkRoles[Roles.EDIT_ENTRIES])
            {
                yield return new NavItem
                {
                    Id = "9",
                    Href = "/Admin/Entries",
                    IconName = IconName.Folder2Open,
                    Text = "Formulareinträge",
                    ParentId = "7"
                };
            }







            var user = await authService.GetUserAsync();


            if (user is not null)
            {
                yield return new NavItem
                {
                    Id = "14",
                    IconName = IconName.PersonCircle,
                    Text = "Account",
                    IconColor = IconColor.Success
                };

                yield return new NavItem
                {
                    Id = "15",
                    Href = "/Account/Entries",
                    IconName = IconName.PencilFill,
                    Text = "Meine Formulare",
                    ParentId = "14"
                };

                yield return new NavItem
                {
                    Id = "16",
                    Href = "/Account/Assigned",
                    IconName = IconName.Send,
                    Text = "Mir zugeordnet",
                    ParentId = "14"
                };
                yield return new NavItem
                {
                    Id = "17",
                    Href = "/Account/Details",
                    IconName = IconName.PersonVCard,
                    Text = "Accountdetails",
                    ParentId = "14"
                };
                yield return new NavItem
                {
                    Id = "18",
                    Href = "/Account/Logout",
                    IconName = IconName.BoxArrowInRight,
                    Text = "Logout",
                    ParentId = "14",
                    IconColor = IconColor.Danger
                };
            }
            else
            {
                yield return new NavItem
                {
                    Id = "18",
                    Href = "/Account/Login",
                    IconName = IconName.BoxArrowInRight,
                    Text = "Login",
                    IconColor = IconColor.Success
                };
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                bool isTablet = await JSRuntime.InvokeAsync<bool>("blazorHelpers.isTablet");

                if (isTablet)
                {
                    ToggleSidebar();
                }
            }

        }
        private void ToggleSidebar()
        {
            _sidebar.ToggleSidebar();
        }
    }
}