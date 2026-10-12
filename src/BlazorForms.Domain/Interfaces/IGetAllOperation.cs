using System.Data;

namespace BlazorForms.Domain;

public interface IGetAllOperation<TObject>
{
    /// <summary>
    /// Gets all objects from the database.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A list of all objects.
    /// </returns>
    static abstract Task<IEnumerable<TObject>> GetAllAsync(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
}
