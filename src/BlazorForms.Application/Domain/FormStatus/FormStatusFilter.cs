namespace BlazorForms.Application.Domain;

public class FormStatusFilter : PageFilterBase
{
    public override Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "SEARCH_PHRASE", $"%{SearchPhrase}%".ToUpper() }
        };
    }
}
