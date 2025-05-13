using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BlazorForms.Application.Domain;

public class UserRepository : IModelService<User, int?, UserFilter>
{
    public Task<User?> GetAsync(int? userId, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        if(userId is null)
        {
            return Task.FromResult<User?>(null);
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


        return connection.QueryFirstOrDefaultAsync<User>(command);
    }

    public Task<User?> GetByUsernameAsync(string username, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
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


        return connection.QueryFirstOrDefaultAsync<User>(command);
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
    }

    public Task DeleteAsync(User input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResponse<User>> GetAsync(UserFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string GetWhereClause()
        {
            return "WHERE 1 = 1";
        }

        StringBuilder sb = new();
        sb.AppendLine($"SELECT * FROM users {GetWhereClause()}");
        sb.AppendLine("ORDER BY user_id DESC");
        sb.AppendLine($"LIMIT {(filter.PageNumber - 1) * filter.Limit}, {filter.Limit}");
        return null;
    }
}

