namespace BlazorForms.Domain;

public sealed class FormTextElement : FormTextElementBase
{
    public override FormElementType GetElementType() => FormElementType.Text;
}
