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

        return connection.QueryFirstOrDefaultAsync<User>(command);
    }

    public async Task CreateAsync(User input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            INSERT INTO users
            (
                user_group_id,
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
                @USER_GROUP_ID,
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
                user_group_id = @USER_GROUP_ID,
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

    public async Task<PagedResponse<User>> GetAsync(UserFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {

        string sql =
            $"""
            SELECT 
                * 
            FROM users
            WHERE 1 = 1 {GetFilterWhere(filter)}  
            ORDER BY user_id DESC
            LIMIT {filter.Limit} OFFSET {(filter.PageNumber - 1) * filter.Limit} 
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
        var total = await GetTotalAsync(filter, connection, transaction, cancellationToken);

        var response = new PagedResponse<User>
        {
            Items = results.AsList(),
            Page = filter.PageNumber,
            PageSize = filter.Limit,
            Total = total
        };

        return response;
    }

    public string GetFilterWhere(UserFilter filter)
    {
        StringBuilder sb = new();
        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@" AND 
(
        UPPER(display_name) LIKE @SEARCH_PHRASE
    OR  UPPER(email) LIKE @SEARCH_PHRASE
    OR  UPPER(username) LIKE @SEARCH_PHRASE
)");
        }

        string sql = sb.ToString();
        return sql;
    }

    public Task<int> GetTotalAsync(UserFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            $"""
            SELECT 
                COUNT(*) 
            FROM users
            WHERE 1 = 1 {GetFilterWhere(filter)}
            """;

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: filter.GetParameters(),
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        return connection.ExecuteScalarAsync<int>(command);
    }
}

