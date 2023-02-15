using DatabaseControllerProvider;

namespace FormPortal.Core.Interfaces
{
    /// <summary>
    /// Provides generalized CUD operations for an object Service.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IModelService<T>
    {
        /// <summary>
        /// Saves the object as new entry in the database.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbController"></param>
        /// <returns></returns>
        Task CreateAsync(T input, IDbController dbController);
        /// <summary>
        /// Updates an existing entry of the object in the database.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbController"></param>
        /// <returns></returns>
        Task UpdateAsync(T input, IDbController dbController);

        /// <summary>
        /// Compares two instances of the same object and updates it in the database when the two objects are different from each other.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="oldInputToCompare"
        /// <param name="dbController"></param>
        /// <returns></returns>
        Task UpdateAsync(T input, T oldInputToCompare, IDbController dbController);

        /// <summary>
        /// Deletes the object from the database.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbController"></param>
        /// <returns></returns>
        Task DeleteAsync(T input, IDbController dbController);
    }
    /// <summary>
    /// <para>
    /// Provides generalized CRUD operations for an object Service.
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="GetKeyIdentifier"></typeparam>
    public interface IModelService<T, TIdentifier> : IModelService<T>
    {
        /// <summary>
        /// Gets the objects from the database
        /// </summary>
        /// <param name="identifier">The unique identifer for the object.</param>
        /// <param name="fbController"></param>
        /// <returns>
        /// If the object does not exist than this method will return NULL.
        /// </returns>
        Task<T?> GetAsync(TIdentifier identifier, IDbController dbController);
    }
    /// <summary>
    /// <inheritdoc />
    /// Expands the CRUD Operations with conditional search filters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TIdentifier"></typeparam>
    /// <typeparam name="TFilter"></typeparam>
    public interface IModelService<T, TIdentifier, TFilter> : IModelService<T, TIdentifier>
    {
        /// <summary>
        /// Gets data from the database based on the provided search filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sqlController"></param>
        /// <returns></returns>
        Task<List<T>> GetAsync(TFilter filter, IDbController dbController);
        /// <summary>
        /// Gets the total amount of search results based on the provided filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sqlController"></param>
        /// <returns></returns>
        Task<int> GetTotalAsync(TFilter filter, IDbController dbController);
        /// <summary>
        /// Generates the conditional WHERE statement for the SQL query.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        string GetFilterWhere(TFilter filter);
        /// <summary>
        /// Gets a dictionary of parameters for the filter which can be used in Dapper-Queries.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Dictionary<string, object?> GetFilterParameter(TFilter filter);
    }
}
