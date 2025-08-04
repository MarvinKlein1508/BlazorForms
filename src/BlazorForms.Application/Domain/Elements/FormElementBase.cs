namespace BlazorForms.Core.Models.Elements;
// TODO: Make abstract again
public class FormElementBase
{
    public int ElementId { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public int FormId { get; set; }
    public int RowId { get; set; }
    public int ColumnId { get; set; }
    public int TableParentElementId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsRequired { get; set; }
    public bool ResetOnCopy { get; set; }
    public int SortOrder { get; set; }
}
