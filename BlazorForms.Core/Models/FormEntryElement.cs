namespace BlazorForms.Core.Models;

public class FormEntryElement
{
    public int EntryId { get; set; }
    public int FormId { get; set; }
    public int ElementId { get; set; }
    public bool ValueBoolean { get; set; }
    public string ValueString { get; set; } = string.Empty;
    public decimal ValueNumber { get; set; }
    public DateTime ValueDate { get; set; }
}
