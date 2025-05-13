
using Dapper;
using System.Data;

namespace BlazorForms.Application.Domain;

public static class UserRoleRepository
{
    public static Task CreateAsync(UserRole input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            INSERT INTO user_roles
            (
                user_id,
                role_id,
                is_active
            )
            VALUES
            (
                @USER_ID,
                @ROLE_ID,
                @IS_ACTIVE
            )
            """;

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: input.GetParameters(),
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        return connection.ExecuteAsync(command);
    }

    public static Task CleanAsync(int userId, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM user_roles WHERE user_id = @USER_ID";
        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: new { USER_ID = userId },
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        return connection.ExecuteAsync(command);
    }

    public static async Task<List<UserRole>> GetAsync(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default, params int[] userIds)
    {
        if (userIds.Length == 0)
        {
            return [];
        }

        string sql =
            $"""
            SELECT
                ui.user_id,
                r.role_id,
                r.name AS role_name,
                coalesce(ur.is_active, 0) as is_active
            FROM roles r 
            LEFT JOIN (VALUES ({string.Join("),(", userIds)})) AS ui (user_id) ON 1 = 1
            LEFT JOIN user_roles ur ON (ur.user_id = ui.user_id AND ur.role_id = r.role_id)
            """;

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: null,
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        var results = await connection.QueryAsync<UserRole>(command);
        return results.ToList();
    }
}