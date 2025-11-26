using System.Data;
using System.Text;
using Dapper;

namespace BlazorForms.Infrastructure.Repositories;

public class LanguageRepository : IModelService<Language, int?, LanguageFilter>
{
    public Task CreateAsync(Language input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Language input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Language?> GetAsync(int? identifier, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResponse<Language>> GetAsync(LanguageFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            $"""
            SELECT 
                * 
            FROM languages
            WHERE 1 = 1 {GetFilterWhere(filter)}  
            ORDER BY sort_order DESC
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

        var results = await connection.QueryAsync<Language>(command);
        var total = await GetTotalAsync(filter, connection, transaction, cancellationToken);

        var response = new PagedResponse<Language>
        {
            Items = results.AsList(),
            Page = filter.PageNumber,
            PageSize = filter.Limit,
            Total = total
        };

        return response;
    }

    public string GetFilterWhere(LanguageFilter filter)
    {
        StringBuilder sb = new();
        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@" AND 
(
        UPPER(name) LIKE @SEARCH_PHRASE
)");
        }

        string sql = sb.ToString();
        return sql;
    }

    public Task<int> GetTotalAsync(LanguageFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            $"""
            SELECT 
                COUNT(*) 
            FROM languages
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

    public Task UpdateAsync(Language input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public static async Task<List<Language>> GetAllAsync(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql = "SELECT * FROM languages ORDER BY sort_order";

        var result = await connection.QueryAsync<Language>(sql);

        return result.ToList();
    }
}
