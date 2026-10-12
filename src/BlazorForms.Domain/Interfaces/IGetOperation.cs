using System.Data;

namespace BlazorForms.Domain;

/// <summary>
/// Provides a generalized GET function for a specific type.
/// </summary>
/// <typeparam name="TObject"></typeparam>
/// <typeparam name="TIdentifier"></typeparam>
public interface IGetOperation<TObject, TIdentifier>
{
    /// <summary>
    /// Gets the objects from the database
    /// </summary>
    /// <param name="identifier">The unique identifer for the object.</param>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// If the object does not exist than this method will return NULL.
    /// </returns>
    Task<TObject?> GetAsync(TIdentifier identifier, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
}
