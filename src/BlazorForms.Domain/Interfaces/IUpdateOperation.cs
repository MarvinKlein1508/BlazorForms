using System.Data;

namespace BlazorForms.Domain;

/// <summary>
/// Provides a generalized UPDATE function for a specific type.
/// </summary>
/// <typeparam name="TObject"></typeparam>
public interface IUpdateOperation<TObject>
{
    /// <summary>
    /// Updates an existing entry of the object in the database.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateAsync(TObject input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
}
