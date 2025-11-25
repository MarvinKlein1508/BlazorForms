namespace BlazorForms.Domain.Entities.Elements;

public sealed class FormLabelElement : FormElementBase
{
    public string LabelContent { get; set; } = string.Empty;
    public override FormElementType GetElementType() => FormElementType.Label;
}
