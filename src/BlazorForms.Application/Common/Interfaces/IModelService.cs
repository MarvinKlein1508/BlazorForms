using System.Data;

namespace BlazorForms.Application.Common.Interfaces;

/// <summary>
/// Provides generalized CUD operations for an object Service.
/// </summary>
/// <typeparam name="TObject"></typeparam>
public interface IModelService<TObject> : ICreateOperation<TObject>, IUpdateOperation<TObject>, IDeleteOperation<TObject>
{

}

/// <summary>
/// <para>
/// Provides generalized CRUD operations for an object Service.
/// </para>
/// </summary>
/// <typeparam name="TObject"></typeparam>
/// <typeparam name="TIdentifier"></typeparam>
public interface IModelService<TObject, TIdentifier> : IModelService<TObject>, IGetOperation<TObject, TIdentifier>
{

}

/// <summary>
/// <inheritdoc />
/// Expands the CRUD Operations with conditional search filters.
/// </summary>
/// <typeparam name="TObject"></typeparam>
/// <typeparam name="TIdentifier"></typeparam>
/// <typeparam name="TFilter"></typeparam>
public interface IModelService<TObject, TIdentifier, TFilter> : IModelService<TObject, TIdentifier>, IFilterOperations<TObject, TFilter>
{

}

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

    string GetFilterWhere(TFilter filter);
}