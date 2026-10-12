using System.Globalization;

namespace BlazorForms.Domain;

public class FormStatusFilter : PagedFilter, IDbParameterizable
{
    public string? SearchPhrase { get; set; }
    public string LanguageCode => CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "SEARCH_PHRASE", $"%{SearchPhrase}%".ToUpper() },
            { "LANGUAGE_CODE", LanguageCode }
        };
    }
}
