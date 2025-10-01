
namespace BlazorForms.Application.Domain;

public sealed class Config : IDbModel<string>, IDbParameterizable
{
    public static Config DefaultMainConfig => new()
    {
        Code = ConfigRepository.MAIN_CONFIG_CODE,
        DefaultLanguageId = 1
    };

    public required string Code { get; set; }
    public int DefaultLanguageId { get; set; }

    public string GetIdentifier() => Code;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "CODE", Code },
            { "DEFAULT_LANGUAGE_ID", DefaultLanguageId }
        };
    }
}
