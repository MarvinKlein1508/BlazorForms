using Dapper;
using System;
using System.Data;

namespace BlazorForms.Application.Domain;

public class UserRepository 
{
    public Task<User?> GetAsync(int userId, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
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
            parameters: new { GUID = guid },
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        input.UserId = await connection.ExecuteScalarAsync<int>(command);
    }
}

