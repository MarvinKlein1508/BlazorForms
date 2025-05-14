namespace BlazorForms.Application.Common;

public abstract class FilterBase : IDbParameterizable
{
    private string _searchPhrase = string.Empty;
    public string SearchPhrase { get => _searchPhrase; set => _searchPhrase = value?.ToUpper() ?? string.Empty; }
    public abstract Dictionary<string, object?> GetParameters();
    public virtual void Reset()
    {
        SearchPhrase = string.Empty;
    }
}