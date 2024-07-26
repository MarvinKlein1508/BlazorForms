using BlazorForms.Core.Constants;
using BlazorForms.Core.Models;
using BlazorForms.Core.Services;
using Microsoft.JSInterop;

namespace BlazorForms.Components.Layout
{
    public partial class MainLayout : IDisposable
    {
        //private Sidebar _sidebar = default!;
        //private Offcanvas _offcanvas = default!;
        //IEnumerable<NavItem> _navItems = default!;

        public User? User { get; set; }
        private List<Notification> _notifications = [];

        protected override async Task OnInitializedAsync()
        {
            User = await authService.GetUserAsync();

            NotificationService.NotificationsChanged += NotificationService_NotificationsChanged;
        }

        private async Task NotificationService_NotificationsChanged()
        {
            if (User is not null)
            {
                _notifications = notificationService.GetNotifications(User).ToList();
            }

            await InvokeAsync(StateHasChanged);
        }

        //private async Task<SidebarDataProviderResult> SidebarDataProvider(SidebarDataProviderRequest request)
        //{
        //    _navItems ??= await GetNavItems().ToListAsync();
        //    return await Task.FromResult(request.ApplyTo(_navItems));
        //}

        //private async IAsyncEnumerable<NavItem> GetNavItems()
        //{
        //    yield return new NavItem
        //    {
        //        Id = "1",
        //        Href = "/",
        //        IconName = IconName.HouseDoorFill,
        //        Text = localizer["NAV_DASHBOARD"]
        //    };

        //    var checkRoles = await authService.CheckRoles(Roles.EDIT_FORMS, Roles.EDIT_ENTRIES, Roles.EDIT_USERS, Roles.EDIT_STATUS);


        //    if (checkRoles.Any(x => x.Value))
        //    {
        //        yield return new NavItem
        //        {
        //            Id = "7",
        //            IconName = IconName.Gear,
        //            Text = localizer["NAV_ADMIN"],
        //            IconColor = IconColor.Success
        //        };
        //    }

        //    if (checkRoles[Roles.EDIT_USERS])
        //    {
        //        yield return new NavItem
        //        {
        //            Id = "8",
        //            Href = "/Admin/Users",
        //            IconName = IconName.PeopleFill,
        //            Text = localizer["NAV_ADMIN_USERS"],
        //            ParentId = "7"
        //        };
        //    }

        //    if (checkRoles[Roles.EDIT_STATUS])
        //    {
        //        yield return new NavItem
        //        {
        //            Id = "11",
        //            Href = "/Admin/Status",
        //            IconName = IconName.ListCheck,
        //            Text = localizer["NAV_ADMIN_STATUS"],
        //            ParentId = "7"
        //        };
        //    }

        //    if (checkRoles[Roles.EDIT_FORMS])
        //    {
        //        yield return new NavItem
        //        {
        //            Id = "10",
        //            Href = "/Admin/Forms",
        //            IconName = IconName.Files,
        //            Text = localizer["NAV_ADMIN_FORMS"],
        //            ParentId = "7"
        //        };
        //    }

        //    if (checkRoles[Roles.EDIT_ENTRIES])
        //    {
        //        yield return new NavItem
        //        {
        //            Id = "9",
        //            Href = "/Admin/Entries",
        //            IconName = IconName.Folder2Open,
        //            Text = localizer["NAV_ADMIN_FORM_ENTRIES"],
        //            ParentId = "7"
        //        };
        //    }

   



        //}

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                bool isTablet = await JSRuntime.InvokeAsync<bool>("blazorHelpers.isTablet");

                if (isTablet)
                {
                    //ToggleSidebar();
                }
            }

        }
        //private void ToggleSidebar()
        //{
        //    _sidebar.ToggleSidebar();
        //}

        public void Dispose()
        {
            NotificationService.NotificationsChanged -= NotificationService_NotificationsChanged;
        }
    }
}