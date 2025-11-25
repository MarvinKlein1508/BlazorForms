namespace BlazorForms.Domain.Entities.Elements;

public sealed class FormTextAreaElement : FormTextElementBase
{
    public override FormElementType GetElementType() => FormElementType.TextArea;
}
