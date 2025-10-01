using Dapper;
using System.Data;

namespace BlazorForms.Application.Domain;

public class ConfigRepository : IUpdateOperation<Config>
{
    public const string MAIN_CONFIG_CODE = "main";
    public static Task<Config> GetMainConfig(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql = "SELECT * FROM configs WHERE code = @CODE";

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: new
            {
                CODE = MAIN_CONFIG_CODE
            },
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        return connection.QueryFirstAsync<Config>(command);
    }

    public async Task UpdateAsync(Config input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE configs SET
                default_language_id = @DEFAULT_LANGUAGE_ID
            WHERE 
                code = @CODE
            """;

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: input.GetParameters(),
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        await connection.ExecuteAsync(command);
    }
}
