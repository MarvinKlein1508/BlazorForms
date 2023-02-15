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
}
