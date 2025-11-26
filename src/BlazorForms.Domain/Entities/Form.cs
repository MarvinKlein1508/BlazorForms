using BlazorForms.Domain.Entities.Elements;

namespace BlazorForms.Domain.Entities;

public class Form
{
    public int FormId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DefaultName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int DefaultStatusId { get; set; }
    public int LanguageId { get; set; }
    public byte[] Logo { get; set; } = [];
    public byte[] Image { get; set; } = [];
    public int SortOrder { get; set; }
    public List<FormRow> Rows { get; set; } = [];

    public void RemoveRow(FormRow row)
    {
        Rows.Remove(row);
    }

    public void RemoveColumn(FormColumn column)
    {
        foreach (var row in Rows)
        {
            row.Columns.Remove(column);
        }
    }

    public void RemoveElement(FormElementBase element)
    {
        foreach (var column in Rows.SelectMany(r => r.Columns))
        {
            if (!column.Elements.Contains(element))
            {
                continue;
            }

            column.Elements.Remove(element);
            return;
        }
    }

    public void RemoveEmptyRows()
    {
        var list = Rows.Where(x => x.Columns.Count == 0).ToArray();
        foreach (var item in list)
        {
            Rows.Remove(item);
        }
    }
}
