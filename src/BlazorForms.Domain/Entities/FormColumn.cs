namespace BlazorForms.Domain;

public class FormColumn : IDbModel<Guid>
{
    public Guid ColumnId { get; set; }
    public List<FormElement> Elements { get; set; } = [];
    public Guid GetIdentifier() => ColumnId;
}
