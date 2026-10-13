namespace BlazorForms.Domain;

public abstract class FormElement : IDbModel<Guid>
{
    public Guid ElementId { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }
    public Guid GetIdentifier() => ElementId;
    public abstract ElementType GetElementType();
}
