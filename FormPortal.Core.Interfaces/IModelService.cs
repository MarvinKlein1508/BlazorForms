using DatabaseControllerProvider;

namespace FormPortal.Core.Interfaces
{
    /// <summary>
    /// Standartisiert die CRUD-Operations von Model-Services 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IModelService<T>
    {
        /// <summary>
        /// Speichert das Objekt in der Datenbank als neuen Datensatz.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbController"></param>
        /// <returns></returns>
        Task CreateAsync(T input, IDbController dbController);
        /// <summary>
        /// Aktualisiert den Datensatz für das Objekt in der Datenbank
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbController"></param>
        /// <returns></returns>
        Task UpdateAsync(T input, IDbController dbController);

        /// <summary>
        /// Aktualisiert den Datensatz für das Objekt in der Datenbank
        /// </summary>
        /// <param name="input"></param>
        /// <param name="oldInputToCompare"
        /// <param name="dbController"></param>
        /// <returns></returns>
        Task UpdateAsync(T input, T oldInputToCompare, IDbController dbController);

        /// <summary>
        /// Löscht den Datensatz für das Objekt aus der Datenbank
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbController"></param>
        /// <returns></returns>
        Task DeleteAsync(T input, IDbController dbController);
    }
    /// <summary>
    /// <inheritdoc/>
    /// <para>
    /// Erweitert die Standard CRUD Operations noch um das Laden eines einzelnen Objektes.
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="GetKeyIdentifier"></typeparam>
    public interface IModelService<T, TIdentifier> : IModelService<T>
    {
        /// <summary>
        /// Lädt ein Objekt aus der Datenbank für einen besteimmten Identifier
        /// </summary>
        /// <param name="identifier">Identifier, wie das Objekt in der Datenbank identifiziert werden kann.</param>
        /// <param name="fbController"></param>
        /// <returns>
        /// Wenn das Objekt in der Datenbank für den Identifier existiert, dann wird dieses zurückgeben. Ansonsten wird null zurückgegeben.
        /// </returns>
        Task<T?> GetAsync(TIdentifier identifier, IDbController dbController);
    }
    /// <summary>
    /// Erweitert die Standard CRUD Operations um eine Suchmöglichkeit mit Filtern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TIdentifier"></typeparam>
    /// <typeparam name="TFilter"></typeparam>
    public interface IModelService<T, TIdentifier, TFilter> : IModelService<T, TIdentifier>
    {
        /// <summary>
        /// Lädt Daten auf Basis des übergebenen Filters.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sqlController"></param>
        /// <returns></returns>
        Task<List<T>> GetAsync(TFilter filter, IDbController dbController);
        /// <summary>
        /// Ruft die Anzahl der Ergebnisse auf Basis des Filters ab.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sqlController"></param>
        /// <returns></returns>
        Task<int> GetTotalAsync(TFilter filter, IDbController dbController);
        /// <summary>
        /// Generiert für einen Filter die SQL-WHERE Klausel
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        string GetFilterWhere(TFilter filter);
        /// <summary>
        /// Liefert für einen Filter ein Objekt mit allen Parametern für die Datenbankabfrage mit Dapper zurück.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Dictionary<string, object?> GetFilterParameter(TFilter filter);
    }
}
