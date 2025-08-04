namespace BlazorForms.Application.Domain.Elements;

public sealed class FormRadioElement : FormElementBase
{
    public List<FormElementOption> Options { get; set; } = [];
    public override FormElementType GetElementType() => FormElementType.Radio;
}