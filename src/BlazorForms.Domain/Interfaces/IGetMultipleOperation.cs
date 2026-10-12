using System.Data;

namespace BlazorForms.Domain;

/// <summary>
/// Defines a contract for retrieving multiple objects from a data source using their unique identifiers.   
/// </summary>
/// <remarks>Implementations should ensure that all provided identifiers are valid and handle cases where some or
/// all identifiers do not correspond to existing objects. This interface is typically used in data access layers that
/// require batch retrieval operations.</remarks>
/// <typeparam name="TObject">The type of objects to be retrieved.</typeparam>
/// <typeparam name="TIdentifier">The type of the unique identifiers used to locate the objects.</typeparam>
public interface IGetMultipleOperation<TObject, TIdentifier>
{
    /// <summary>
    /// Gets multiple objects from the database
    /// </summary>
    /// <param name="identifiers">The unique identifers for the objects.</param>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// If no objects exist than this method will return an empty collection.
    /// </returns>
    Task<IEnumerable<TObject>> GetAsync(IEnumerable<TIdentifier> identifiers, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
}
