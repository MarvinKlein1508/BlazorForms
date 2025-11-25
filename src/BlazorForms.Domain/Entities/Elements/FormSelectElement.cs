namespace BlazorForms.Domain.Entities.Elements;

public sealed class FormSelectElement : FormElementBase
{
    public List<FormElementOption> Options { get; set; } = [];
    public override FormElementType GetElementType() => FormElementType.Select;
}
