

namespace BlazorForms.Application.Domain;

public class UserFilter : PageFilterBase, IDbParameterizable
{
    public Dictionary<string, object?> GetParameters()
    {
        return [];
    }
}