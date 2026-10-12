namespace BlazorForms.Domain;

public sealed class FormCheckboxElement : FormElementBase
{
    public override FormElementType GetElementType() => FormElementType.Checkbox;
}
