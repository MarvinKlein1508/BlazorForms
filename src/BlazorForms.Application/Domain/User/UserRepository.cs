using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BlazorForms.Application.Domain;

public class UserRepository : IModelService<User, int?, UserFilter>
{
    public async Task<User?> GetAsync(int? userId, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        if (userId is null)
        {
            return null;
        }

        string sql = "SELECT * FROM users WHERE user_id = @USER_ID";

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: new { USER_ID = userId },
            transaction: transaction,
            cancellationToken: cancellationToken
        );


        var result = await connection.QueryFirstOrDefaultAsync<User>(command);

        if (result is null)
        {
            return null;
        }

        result.Roles = await UserRoleRepository.GetAsync(connection, transaction, cancellationToken, result.UserId);

        return result;
    }

    public async Task<User?> GetByUsernameAsync(string username, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql = "SELECT * FROM users WHERE UPPER(username) = @USERNAME";

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: new { USERNAME = username },
            transaction: transaction,
            cancellationToken: cancellationToken
        );


        var result = await connection.QueryFirstOrDefaultAsync<User>(command);

        if (result is null)
        {
            return null;
        }

        result.Roles = await UserRoleRepository.GetAsync(connection, transaction, cancellationToken, result.UserId);

        return result;
    }

    public Task<User?> GetByActiveDirectoryGuid(Guid guid, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql = "SELECT * FROM users WHERE active_directory_guid = @GUID";

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: new { GUID = guid },
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        // We don't need to load roles here because they will be taken from the Active Directory on login

        return connection.QueryFirstOrDefaultAsync<User>(command);
    }

    public async Task CreateAsync(User input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            INSERT INTO users
            (
                username,
                display_name,
                active_directory_guid,
                email,
                password,
                salt,
                origin
            )
            VALUES 
            (
                @USERNAME,
                @DISPLAY_NAME,
                @ACTIVE_DIRECTORY_GUID,
                @EMAIL,
                @PASSWORD,
                @SALT,
                @ORIGIN
            ) RETURNING user_id
            """;

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: input.GetParameters(),
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        input.UserId = await connection.ExecuteScalarAsync<int>(command);

        foreach (var item in input.Roles)
        {
            item.UserId = input.UserId;
            await UserRoleRepository.CreateAsync(item, connection, transaction, cancellationToken);
        }
    }

    public async Task UpdateAsync(User input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            UPDATE users SET
                username = @USERNAME,
                display_name = @DISPLAY_NAME,
                email = @EMAIL
            WHERE 
                user_id = @USER_ID
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

        await UserRoleRepository.CleanAsync(input.UserId, connection, transaction, cancellationToken);

        foreach (var item in input.Roles)
        {
            item.UserId = input.UserId;
            await UserRoleRepository.CreateAsync(item, connection, transaction, cancellationToken);
        }
    }

    public Task DeleteAsync(User input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResponse<User>> GetAsync(UserFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {

        string sql =
            $"""
            SELECT * FROM users
            WHERE 1 = 1 {GetFilterWhere(filter)}  
            ORDER BY user_id DESC
            LIMIT {(filter.PageNumber - 1) * filter.Limit}, {filter.Limit}
            """;

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: filter.GetParameters(),
            transaction: transaction,
            cancellationToken: cancellationToken
        );


        var results = await connection.QueryAsync<User>(command);


        var response = new PagedResponse<User>
        {
            Items = results.ToList(),
            Page = filter.PageNumber,
            PageSize = filter.Limit,
            Total = results.Count()
        };

        return null;
    }

    public string GetFilterWhere(UserFilter filter)
    {
        throw new NotImplementedException();
    }
}

