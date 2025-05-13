using Dapper;
using System.Data;

namespace BlazorForms.Application.Domain;

public class LanguageRepository
{
    public static async Task<List<Language>> GetAllAsync(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql = "SELECT * FROM languages ORDER BY sort_order";

        var result = await connection.QueryAsync<Language>(sql);

        return result.ToList();
    }
}