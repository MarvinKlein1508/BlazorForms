namespace BlazorForms.Application.Domain.Elements;

public sealed class FormTextAreaElement : FormElementBase
{
    public string? RegexPattern { get; set; }
    public string? RegexValidationMessage { get; set; }
    public int TextMinLength { get; set; }
    public int TextMaxLength { get; set; }
    public override FormElementType GetElementType() => FormElementType.TextArea;
}