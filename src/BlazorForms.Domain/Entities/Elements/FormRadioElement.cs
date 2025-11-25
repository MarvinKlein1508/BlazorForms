namespace BlazorForms.Domain.Entities.Elements;

public sealed class FormRadioElement : FormElementBase
{
    public List<FormElementOption> Options { get; set; } = [];
    public override FormElementType GetElementType() => FormElementType.Radio;
}
