using System.Data;

namespace BlazorForms.Domain;

/// <summary>
/// Provides a generalized CREATE function for a specific type.
/// </summary>
/// <typeparam name="TObject"></typeparam>
public interface ICreateOperation<TObject>
{
    /// <summary>
    /// Saves the object as new entry in the database.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateAsync(TObject input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
}
