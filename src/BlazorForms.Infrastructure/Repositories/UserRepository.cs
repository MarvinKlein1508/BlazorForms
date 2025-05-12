using BlazorForms.Domain.Entities;
using BlazorForms.Domain.Interfaces;
using Dapper;
using System.Data;

namespace BlazorForms.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    public Task<User?> GetAsync(int userId, IDbConnection connection, IDbTransaction? dbTransaction = null, CancellationToken cancellationToken = default)
    {
        string sql = "SELECT * FROM users WHERE user_id = @USER_ID";

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: new { USER_ID = userId },
            transaction: dbTransaction,
            cancellationToken: cancellationToken
        );


        return connection.QueryFirstOrDefaultAsync<User>(command);
    }
}

