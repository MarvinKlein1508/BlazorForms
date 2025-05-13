using Dapper;
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
}

