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

    public Form? Form { get; set; }

    /// <summary>
    /// Creates an empty row.
    /// </summary>
    public FormRow()
    {

    }
    /// <summary>
    /// Creates a new row with a specified amount of columns.
    /// </summary>
    /// <param name="columns"></param>
    public FormRow(Form form, int columns)
    {
        Form = form;
        for (int i = 0; i < columns; i++)
        {
            var column = new FormColumn(form)
            {
                Parent = this
            };

            Columns.Add(column);
        }
    }
}
