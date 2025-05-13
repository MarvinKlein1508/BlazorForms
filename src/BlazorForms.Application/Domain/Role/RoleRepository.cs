using Dapper;
using System.Data;

namespace BlazorForms.Application.Domain;

public class RoleRepository : IModelService<Role, int>
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
        }
    }

    public Task DeleteAsync(Role input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Role?> GetAsync(int identifier, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Role input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}