namespace BlazorForms.Application.Domain.Elements;

public sealed class FormLabelElement : FormElementBase
{
    public override FormElementType GetElementType() => FormElementType.Label;
}