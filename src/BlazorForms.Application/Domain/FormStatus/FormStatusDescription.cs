
namespace BlazorForms.Application.Domain;

public class FormStatusDescription : ILocalizationHelper, IDbParameterizable
{
    public int StatusId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string GetLanguageCode() => Code;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "STATUS_ID", StatusId },
            { "CODE", Code },
            { "NAME", Name },
            { "DESCRIPTION", Description }
        };
    }
}
