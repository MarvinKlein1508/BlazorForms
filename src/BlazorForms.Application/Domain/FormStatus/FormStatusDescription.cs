
namespace BlazorForms.Application.Domain;

public class FormStatusDescription : ILocalizationHelper, IDbParameterizable
{
    public int StatusId { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int GetLanguageId() => LanguageId;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "STATUS_ID", StatusId },
            { "LANGUAGE_ID", LanguageId },
            { "NAME", Name },
            { "DESCRIPTION", Description }
        };
    }
}
