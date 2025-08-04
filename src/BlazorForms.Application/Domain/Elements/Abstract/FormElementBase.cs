namespace BlazorForms.Application.Domain.Elements;
// TODO: Make abstract again
public abstract class FormElementBase
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
    public bool ShowOnPdf { get; set; }
    public int SortOrder { get; set; }
    public abstract FormElementType GetElementType();
}
public enum FormElementType
{
    Label = 1,
    Text = 2,
    TextArea = 3,
    Number = 4,
    Select = 5,
    Radio = 6,
    Checkbox = 7,
    Date = 8,
    File = 9,
    Table = 10,
}