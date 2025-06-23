using BlazorForms.Core.Models.Elements;

namespace BlazorForms.Application.Domain;

/// <summary>
/// Represents a column for a <see cref="FormRow"/>
/// </summary>
public class FormColumn
{
    public int Test { get; set; }
    public int ColumnId { get; set; }
    public int FormId { get; set; }
    public int RowId { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    public List<FormElementBase> Elements { get; set; } = [];
}
