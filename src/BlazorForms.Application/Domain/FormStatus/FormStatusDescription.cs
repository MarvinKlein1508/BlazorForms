namespace BlazorForms.Application.Domain;

public class FormStatusDescription : ILocalizationHelper
{
    public int StatusId { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int GetLanguageId() => LanguageId;
}
