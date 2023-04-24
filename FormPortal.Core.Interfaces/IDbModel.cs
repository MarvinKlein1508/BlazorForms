using System.Globalization;


namespace FormPortal.Core.Interfaces
{
    public interface IDbModel
    {
        /// <summary>
        /// Gets the unique database identifier for the object.
        /// </summary>
        int Id { get; }
        /// <summary>
        /// Gets a dictionary of parameters for the object which can be used in Dapper-Queries.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, object?> GetParameters();
    }
    /// <summary>
    /// Defines a database model which supports one or more localizable properties.
    /// </summary>
    public interface ILocalizedDbModel : IDbModel
    {
        /// <summary>
        /// Returns an <see cref="Dictionary{TKey, TValue}"/> of parameters for each available localization.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Dictionary<string, object?>> GetLocalizedParameters();
    }
}
