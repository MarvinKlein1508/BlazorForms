using Dapper;
using System.Data;

namespace BlazorForms.Application.Domain;

public static class RolePermissionRepository 
{
    public static async Task CreateAsync(RolePermission input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        //string sql = "INSERT INTO role_permissions (role_id, permission_id) VALUES (@ROLE_ID, @PERMISSION_ID)";
        //var command = new CommandDefinition
        //(
        //    commandText: sql,
        //    commandType: CommandType.Text,
        //    parameters: new { ROLE_ID = input.RoleId, PERMISSION_ID = input.PermissionId },
        //    transaction: transaction,
        //    cancellationToken: cancellationToken
        //);
        //await connection.ExecuteAsync(command);
    }
}