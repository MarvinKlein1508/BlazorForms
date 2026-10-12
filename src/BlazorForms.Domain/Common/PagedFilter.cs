namespace BlazorForms.Domain;

/// <summary>
/// Represents a base filter with pagination support.
/// </summary>
public abstract class PagedFilter
{
    /// <summary>
    /// Gets or sets the page number to retrieve.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 30;
}
