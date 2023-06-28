namespace BlazorForms.Core.Interfaces
{
    /// <summary>
    /// Adds a property to sort a collection of elements.
    /// </summary>
    public interface IHasSortableElement
    {
        int SortOrder { get; set; }
    }
}
