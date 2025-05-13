using Dapper;
using System.Data;

namespace BlazorForms.Application.Domain;

public class RoleRepository : IModelService<Role, int?>
{
    public async Task CreateAsync(Role input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            INSERT INTO roles
            (
                name,
                active_directory_group_cn,
                can_be_deleted
            )
            VALUES
            (
                @NAME,
                @ACTIVE_DIRECTORY_GROUP_CN,
                @CAN_BE_DELETED
            ) RETURNING role_id
            """;

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: input.GetParameters(),
            transaction: transaction,
            cancellationToken: cancellationToken
        );


        input.RoleId = await connection.ExecuteScalarAsync<int>(command);

        foreach (var permission in input.Permissions)
        {
            permission.RoleId = input.RoleId;
            await RolePermissionRepository.CreateAsync(permission, connection, transaction, cancellationToken);
        }
    }

    public Task DeleteAsync(Role input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Role?> GetAsync(int? identifier, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        if (identifier is null)
        {
            return null;
        }
       
        string sql = "SELECT * FROM roles WHERE role_id = @ROLE_ID";

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: new { ROLE_ID = identifier },
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryFirstOrDefaultAsync<Role>(command);

        if (result is not null)
        {
            result.Permissions = await RolePermissionRepository.GetAsync(connection, transaction, cancellationToken, result.RoleId);
        }

        return result;
    }

    public async Task UpdateAsync(Role input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            UPDATE role SET
                name = @NAME,
                active_directory_group_cn = @ACTIVE_DIRECTORY_GROUP_CN
            WHERE
                role_id = @ROLE_ID
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

        await RolePermissionRepository.CleanAsync(input.RoleId, connection, transaction, cancellationToken);

        foreach (var permission in input.Permissions)
        {
            permission.RoleId = input.RoleId;
            await RolePermissionRepository.CreateAsync(permission, connection, transaction, cancellationToken);
        }
    }
}