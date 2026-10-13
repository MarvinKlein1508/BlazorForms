namespace BlazorForms.Domain;

public class Form : IDbModel<Guid>
{
    public Guid FormId { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public List<FormRow> Rows { get; set; } = [];

    public Guid GetIdentifier() => FormId;
}
