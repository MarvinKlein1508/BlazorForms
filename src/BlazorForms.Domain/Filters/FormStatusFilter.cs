using System.Globalization;

namespace BlazorForms.Domain.Filters;

public class FormStatusFilter : PageFilterBase
{
    public string LanguageCode => CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
    public override Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "SEARCH_PHRASE", $"%{SearchPhrase}%".ToUpper() },
            { "LANGUAGE_CODE", LanguageCode }
        };
    }
}
