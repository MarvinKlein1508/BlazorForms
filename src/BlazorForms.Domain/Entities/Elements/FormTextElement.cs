namespace BlazorForms.Domain.Entities.Elements;

public sealed class FormTextElement : FormTextElementBase
{
    public override FormElementType GetElementType() => FormElementType.Text;
}
