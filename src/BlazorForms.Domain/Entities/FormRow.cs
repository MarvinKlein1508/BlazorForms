namespace BlazorForms.Domain;

public class FormRow : IDbModel<Guid>
{
    public Guid RowId { get; set; }
    public List<FormColumn> Columns { get; set; } = [];
    public Guid GetIdentifier() => RowId;
}
