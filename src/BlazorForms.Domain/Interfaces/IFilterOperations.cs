using System.Data;


namespace BlazorForms.Domain;

/// <summary>
/// Interface to provide filter methods to any service.
/// </summary>
/// <typeparam name="TObject"></typeparam>
/// <typeparam name="TFilter">A class which holds properties to specify an SQL filter.</typeparam>
public interface IFilterOperations<TObject, TFilter>
{
    /// <summary>
    /// Gets data from the database based on the provided search filter.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PagedResponse<TObject>> GetAsync(TFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
    /// <summary>
    /// Gets the total amount of search results based on the provided filter.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> GetTotalAsync(TFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
    /// <summary>
    /// Generates the conditional WHERE statement for the SQL query.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    string GetFilterWhere(TFilter filter);
}
