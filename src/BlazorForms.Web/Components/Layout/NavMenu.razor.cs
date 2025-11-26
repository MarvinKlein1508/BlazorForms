using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorForms.Web.Components.Layout;

public partial class NavMenu
{
    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Parameter]
    public FluentLayoutHamburger? Hamburger { get; set; }

    public IEnumerable<NavItem> NavItems { get; set; } = [];

    public NavMenu()
    {
        NavItems =
        [
            new NavItem
            {
                Href = "/",
                Text = "Home",
            },
            new NavItem
            {
                Text = "Administration",
                ChildItems =
                [
                    new NavItem
                    {
                        Href = "Admin/Users",
                        Text = "Benutzer"
                    },
                    new NavItem
                    {
                        Href = "Admin/Languages",
                        Text = "Sprachen"
                    },
                    new NavItem
                    {
                        Href = "Admin/Status",
                        Text = "Statusverwaltung"
                    },
                    new NavItem
                    {
                        Href = "Admin/Forms",
                        Text = "Formulare"
                    },
                    new NavItem
                    {
                        Href = "Admin/Entries",
                        Text = "Formulareinträge"
                    }
                ]
            }
        ];
    }
}
