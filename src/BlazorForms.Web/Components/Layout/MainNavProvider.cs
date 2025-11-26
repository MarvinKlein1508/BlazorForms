using Microsoft.AspNetCore.Components.Routing;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace BlazorForms.Web.Components.Layout;

public class MainNavProvider
{
    public IReadOnlyList<NavItem> NavMenuItems { get; init; }

    public MainNavProvider()
    {
        NavMenuItems =
        [
            new NavLink(
                href: "",
                match: NavLinkMatch.All,
                icon: new Icons.Regular.Size20.Home(),
                title: "Home"
            ),
            new NavLink(
                href: "counter",
                match: NavLinkMatch.All,
                icon: new Icons.Regular.Size20.Button(),
                title: "Counter"
            ),
            new NavLink(
                href: "weather",
                match: NavLinkMatch.All,
                icon: new Icons.Regular.Size20.Table(),
                title: "Weather"
            ),
        ];
    }
}
