namespace BlazorForms.Domain;

public sealed class FormTextAreaElement : FormTextElementBase
{
    public override FormElementType GetElementType() => FormElementType.TextArea;
}
