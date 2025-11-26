namespace BlazorForms.Domain.Filters;

public abstract class FilterBase : IDbParameterizable
{
    public string SearchPhrase { get => field ?? string.Empty; set => field = value; }
    public abstract Dictionary<string, object?> GetParameters();
    public virtual void Reset()
    {
        SearchPhrase = string.Empty;
    }
}
