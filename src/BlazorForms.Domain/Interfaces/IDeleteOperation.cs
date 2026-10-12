using System.Data;

namespace BlazorForms.Domain;

/// <summary>
/// Provides a generalized DELETE function for a specific type.
/// </summary>
/// <typeparam name="TObject"></typeparam>
public interface IDeleteOperation<TObject>
{
    /// <summary>
    /// Deletes the object from the database.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(TObject input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
}
