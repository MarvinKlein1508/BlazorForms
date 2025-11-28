using System.Data;
using System.Text;
using Dapper;

namespace BlazorForms.Infrastructure.Repositories;

public sealed class FormRepository(FormRowRepository _formRowRepository) : IModelService<Form, int?, FormFilter>
{
    public Task CreateAsync(Form input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Form input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Form?> GetAsync(int? identifier, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResponse<Form>> GetAsync(FormFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            $"""
            SELECT 
                * 
            FROM forms
            WHERE 1 = 1 {GetFilterWhere(filter)}  
            ORDER BY form_id DESC
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

        var results = await connection.QueryAsync<Form>(command);
        var total = await GetTotalAsync(filter, connection, transaction, cancellationToken);

        var response = new PagedResponse<Form>
        {
            Items = results.AsList(),
            Page = filter.PageNumber,
            PageSize = filter.Limit,
            Total = total
        };

        return response;
    }

    public string GetFilterWhere(FormFilter filter)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@" AND 
(
    UPPER(name) LIKE @SEARCHPHRASE
)");

        }

        string sql = sb.ToString();
        return sql;
    }

    public Task<int> GetTotalAsync(FormFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
           $"""
            SELECT 
                COUNT(*) 
            FROM forms
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

    public Task UpdateAsync(Form input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
