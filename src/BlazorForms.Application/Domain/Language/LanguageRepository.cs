using Dapper;
using System.Data;

namespace BlazorForms.Application.Domain;

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

    public Task<PagedResponse<Language>> GetAsync(LanguageFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public string GetFilterWhere(LanguageFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTotalAsync(LanguageFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
