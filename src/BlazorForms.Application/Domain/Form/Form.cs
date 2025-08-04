namespace BlazorForms.Application.Domain;
public class Form
{
    public int FormId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int DefaultStatusId { get; set; }
    public int LanguageId { get; set; }
    public byte[] Logo { get; set; } = [];
    public byte[] Image { get; set; } = [];
    public int SortOrder { get; set; }
    public List<FormRow> Rows { get; set; } = [];
}
