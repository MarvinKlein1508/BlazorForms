namespace BlazorForms.Domain.Entities;

/// <summary>
/// Represents a row within the Form.
/// </summary>
public class FormRow
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public int Test { get; set; }
    public int RowId { get; set; }
    public int FormId { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets all columns for this row.
    /// </summary>
    public List<FormColumn> Columns { get; set; } = [];
}
