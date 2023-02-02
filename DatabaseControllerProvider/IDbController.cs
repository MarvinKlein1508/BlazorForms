using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace DatabaseControllerProvider
{
    public interface IDbController : IDisposable
    {
        string CommandText { get; }
        string ConnectionString { get; }
        Task CommitChangesAsync();
        Task<T?> GetFirstAsync<T>(string selectCommand, object? param = null);
        Task QueryAsync(string sqlCommand, object? param = null);
        Task RollbackChangesAsync();
        Task<List<T>> SelectDataAsync<T>(string selectCommand, object? param = null);
        Task StartTransactionAsync();
        string GetLastIdSql();
        string GetPaginationSyntax(int pageNumber, int limit);
    }
    public interface IDbController<TConnection, TCommand, TTransaction> : IDbController where TConnection : IDbConnection where TCommand : IDbCommand where TTransaction : IDbTransaction
    {
        TConnection Connection { get; }
        TTransaction? Transaction { get; }
        TCommand Command { get; }
    }
}