using System.Data;
using System.Text;
using Dapper;

namespace BlazorForms.Infrastructure.Repositories;

/// <summary>
/// Repository for managing <see cref="User"/> entities..
/// </summary>
public class UserRepository : IGetOperation<User, int?>,
    ICreateOperation<User>,
    IUpdateOperation<User>,
    IFilterOperations<User, UserFilter>
{
    /// <summary>
    /// Creates a new <see cref="User"/> in the database and persists associated roles.
    /// After insertion the generated <see cref="User.UserId"/> is assigned to <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The user to create. Associated roles in <see cref="User.Roles"/> will also be created.</param>
    /// <param name="connection">Open database connection to use for the operation.</param>
    /// <param name="transaction">Optional transaction to enlist the inserts in.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous create operation.</returns>
    public async Task CreateAsync(User input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            INSERT INTO users
            (
                username,
                role,
                display_name,
                active_directory_guid,
                email,
                password,
                salt,
                account_type,
                is_active,
                two_factor_enabled,
                lockout_end,
                lockout_enabled,
                access_failed_count,
                two_factor_token
            )
            VALUES
            (
                @USERNAME,
                @ROLE,
                @DISPLAY_NAME,
                @ACTIVE_DIRECTORY_GUID,
                @EMAIL,
                @PASSWORD,
                @SALT,
                @ACCOUNT_TYPE,
                @IS_ACTIVE,
                @TWO_FACTOR_ENABLED,
                @LOCKOUT_END,
                @LOCKOUT_ENABLED,
                @ACCESS_FAILED_COUNT,
                @TWO_FACTOR_TOKEN
            ) RETURNING user_id;
            """;

        var command = new CommandDefinition(commandText: sql, parameters: input.GetParameters(), transaction: transaction, cancellationToken: cancellationToken);

        input.UserId = await connection.ExecuteScalarAsync<int>(command);
    }

    /// <summary>
    /// Retrieves a <see cref="User"/> by its identifier.
    /// Also loads associated roles into <see cref="User.Roles"/>.
    /// </summary>
    /// <param name="identifier">The user id to retrieve. If <c>null</c> result will be <c>null</c>.</param>
    /// <param name="connection">Open database connection to use for the query.</param>
    /// <param name="transaction">Optional transaction to use for the query.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The matching <see cref="User"/> or <c>null</c> when no user exists with the provided id.</returns>
    public async Task<User?> GetAsync(int? identifier, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql = "SELECT * FROM users WHERE user_id = @USER_ID";
        var result = await connection.QueryFirstOrDefaultAsync<User>(sql, new { USER_ID = identifier }, transaction);

        if (result is null)
        {
            return null;
        }

        return result;
    }

    /// <summary>
    /// Retrieves a <see cref="User"/> by its Active Directory GUID.
    /// Does not populate roles; use <see cref="GetAsync"/> for role loading.
    /// </summary>
    /// <param name="activeDirectoryGuid">Active Directory GUID to search for.</param>
    /// <param name="connection">Open database connection to use for the query.</param>
    /// <param name="transaction">Optional transaction to use for the query.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The matching <see cref="User"/> or <c>null</c> when no user exists with the provided GUID.</returns>
    public Task<User?> GetByActiveDirectoryGuidAsync(Guid activeDirectoryGuid, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql = "SELECT * FROM users WHERE active_directory_guid = @ACTIVE_DIRECTORY_GUID";
        return connection.QueryFirstOrDefaultAsync<User>(sql, new { ACTIVE_DIRECTORY_GUID = activeDirectoryGuid }, transaction);
    }

    /// <summary>
    /// Updates an existing <see cref="User"/> record and replaces its roles with the provided collection.
    /// </summary>
    /// <param name="input">The user containing updated values and the roles to persist.</param>
    /// <param name="connection">Open database connection to use for the update.</param>
    /// <param name="transaction">Optional transaction to enlist the update in.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    public Task UpdateAsync(User input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            UPDATE users SET
                username = @USERNAME,
                display_name = @DISPLAY_NAME,
                email = @EMAIL,
                access_failed_count = @ACCESS_FAILED_COUNT,
                is_active = @IS_ACTIVE
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

        return connection.ExecuteAsync(command);
    }

    /// <summary>
    /// Sets two-factor authentication values for the specified user.
    /// Only updates the TwoFactorEnabled and TwoFactorToken columns.
    /// </summary>
    /// <param name="input">The user containing the two-factor values to persist.</param>
    /// <param name="connection">Open database connection to use for the update.</param>
    /// <param name="transaction">Optional transaction to enlist the update in.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    public Task SetTwoFactorAuthenticationAsync(User input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            UPDATE users SET
                two_factor_enabled = @TWO_FACTOR_ENABLED,
                two_factor_token = @TWO_FACTOR_TOKEN
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

        return connection.ExecuteAsync(command);
    }

    /// <summary>
    /// Retrieves a <see cref="User"/> by username and loads associated roles into <see cref="User.Roles"/>.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <param name="connection">Open database connection to use for the query.</param>
    /// <param name="transaction">Optional transaction to use for the query.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The matching <see cref="User"/> or <c>null</c> when no user exists with the provided username.</returns>
    public async Task<User?> GetByUsernameAsync(string username, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql = "SELECT * FROM users WHERE username = @USERNAME";
        var result = await connection.QueryFirstOrDefaultAsync<User>(sql, new { USERNAME = username }, transaction);

        if (result is null)
        {
            return null;
        }

        return result;
    }

    public Task ChangePasswordAsync(User input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            UPDATE users SET
                password = @PASSWORD,
                salt = @SALT
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

        return connection.ExecuteAsync(command);
    }

    /// <inheritdoc />
    public async Task<PagedResponse<User>> GetAsync(UserFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            $"""
            SELECT 
                * 
            FROM users 
            WHERE 1 = 1 {GetFilterWhere(filter)}
            ORDER BY user_id DESC
            OFFSET {(filter.Page - 1) * filter.PageSize} ROWS FETCH NEXT {filter.PageSize} ROWS ONLY
            """;

        var command = new CommandDefinition(commandText: sql, parameters: filter.GetParameters(), transaction: transaction, cancellationToken: cancellationToken);
        var items = await connection.QueryAsync<User>(command);
        var total = await GetTotalAsync(filter, connection, transaction, cancellationToken);

        var response = new PagedResponse<User>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            Total = total
        };

        return response;
    }

    /// <inheritdoc />
    public Task<int> GetTotalAsync(UserFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql = $"SELECT COUNT(user_id) FROM users WHERE 1 = 1 {GetFilterWhere(filter)}";
        var command = new CommandDefinition(commandText: sql, parameters: filter.GetParameters(), transaction: transaction, cancellationToken: cancellationToken);
        return connection.QueryFirstAsync<int>(command);
    }

    /// <inheritdoc />
    public string GetFilterWhere(UserFilter filter)
    {
        StringBuilder sb = new();
        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@" AND 
(
        UPPER(display_name) LIKE @SearchPhrase
    OR  UPPER(email) LIKE @SearchPhrase
    OR  UPPER(username) LIKE @SearchPhrase
)");
        }

        string sql = sb.ToString();
        return sql;
    }
}
