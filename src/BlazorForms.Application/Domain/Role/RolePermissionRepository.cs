using Dapper;
using System.Data;
using System.Threading;

namespace BlazorForms.Application.Domain;

public static class RolePermissionRepository 
{
    public static Task CreateAsync(RolePermission input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            INSERT INTO role_permissions 
            (
                role_id, 
                permission_id,
                is_active
            ) 
            VALUES 
            (
                @ROLE_ID, 
                @PERMISSION_ID,
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

    public static async Task<List<RolePermission>> GetAsync(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default, params int[] roleIds)
    {
        if (roleIds.Length == 0)
        {
            return [];
        }

        string sql =
            $"""
            SELECT
                ri.role_id,
                p.permission_id,
                coalesce(rp.is_active, FALSE) as is_active
            FROM permissions p 
            LEFT JOIN (VALUES ({string.Join("),(", roleIds)})) AS ri (role_id) ON 1 = 1
            LEFT JOIN role_permissions rp ON (rp.role_id = ri.role_id AND rp.permission_id = p.permission_id)
            """;

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: null,
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<RolePermission>(command);

        return result.ToList();
    }

    public static Task CleanAsync(int roleId, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM role_permissions WHERE role_id = @ROLE_ID";

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: new { ROLE_ID = roleId },
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        return connection.ExecuteAsync(command);
    }
}