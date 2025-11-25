namespace BlazorForms.Domain.Filters;

public class LanguageFilter : PageFilterBase
{
    public override Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "SEARCH_PHRASE", $"%{SearchPhrase}%".ToUpper() },
        };
    }
}
