namespace BlazorForms.Domain.Entities.Elements;

public abstract class FormTextElementBase : FormElementBase
{
    public string? RegexPattern { get; set; }
    public string? RegexValidationMessage { get; set; }
    public int TextMinLength { get; set; }
    public int TextMaxLength { get; set; }
}
