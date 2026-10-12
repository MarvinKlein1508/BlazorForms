namespace BlazorForms.Domain;

public class LanguageFilter : PagedFilter, IDbParameterizable
{
    public string? SearchPhrase { get; set; }
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "SEARCH_PHRASE", $"%{SearchPhrase}%".ToUpper() },
        };
    }
}
