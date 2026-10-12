namespace BlazorForms.Domain;

public class UserFilter : PagedFilter, IDbParameterizable
{
    public string? SearchPhrase { get; set; }

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { nameof(SearchPhrase), $"%{SearchPhrase?.ToUpper()}%" }
        };
    }
}
