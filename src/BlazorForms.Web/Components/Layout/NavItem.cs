namespace BlazorForms.Web.Components.Layout;

public class NavItem
{
    /// <summary>
    /// Gets or sets the HyperText Reference (href).
    /// </summary>
    public string? Href { get; set; }

    /// <summary>
    /// Gets or sets the navigation link text.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the collection of child navigation items.
    /// </summary>
    public IEnumerable<NavItem> ChildItems { get; set; } = [];
}
