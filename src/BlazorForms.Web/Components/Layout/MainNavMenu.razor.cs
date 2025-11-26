using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorForms.Web.Components.Layout;

public partial class MainNavMenu
{
    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Parameter]
    public FluentLayoutHamburger? Hamburger { get; set; }

    private string GetNavItemClass(NavItem navItem)
    {
        if (IsNavItemActive(navItem))
        {
            return "nav-item active";
        }

        return "nav-item";
    }

    private bool IsNavItemActive(NavItem navItem)
    {
        // If it's a group, consider it active when any child is active.
        if (navItem is NavGroup group)
        {
            foreach (var child in group.Children)
            {
                if (IsNavItemActive(child))
                {
                    return true;
                }
            }

            return false;
        }

        // current path relative to base (normalized)
        var currentRelative = NavigationManager.ToBaseRelativePath(NavigationManager.Uri).TrimEnd('/');

        // If there's no href (and it's not a group) treat this item as "Home"
        // -> active when current path is root.
        if (string.IsNullOrEmpty(navItem.Href))
        {
            return string.IsNullOrEmpty(currentRelative);
        }

        // Get relative paths (path + query) for target, normalized (no trailing slash)
        var targetAbsolute = NavigationManager.ToAbsoluteUri(navItem.Href).ToString();
        var targetRelative = NavigationManager.ToBaseRelativePath(targetAbsolute).TrimEnd('/');

        // Exact match required
        if (navItem.Match == NavLinkMatch.All)
        {
            return string.Equals(currentRelative, targetRelative, StringComparison.OrdinalIgnoreCase);
        }

        // Prefix match: treat root specially
        if (string.IsNullOrEmpty(targetRelative))
        {
            return string.IsNullOrEmpty(currentRelative);
        }

        // Match if equal or current starts with target + '/'
        return string.Equals(currentRelative, targetRelative, StringComparison.OrdinalIgnoreCase)
            || currentRelative.StartsWith(targetRelative + "/", StringComparison.OrdinalIgnoreCase);
    }
}
