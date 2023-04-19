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
    /// <typeparam name="T"></typeparam>
    public interface ILocalizedDbModel<T> : IDbModel
    {
        List<T> Description { get; }
        /// <summary>
        /// Gets the correct <see cref="Description"/> instance for the specified <see cref="CultureInfo"/>
        /// </summary>
        /// <param name="culture"></param>
        /// <returns>When found an instance of <see cref="T"/>, otherwise null.</returns>
        T? GetLocalization(CultureInfo culture);
        /// <summary>
        /// Returns an <see cref="Dictionary{TKey, TValue}"/> of parameters for each available localization.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Dictionary<string, object?>> GetLocalizedParameters();
    }
}
