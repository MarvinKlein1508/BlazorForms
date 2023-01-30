using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core
{
    public sealed class SqlController : IDisposable
    {
        private bool _disposedValue;
        private SqlConnection Connection => Command.Connection;
        public string CommandText => Command.CommandText ?? String.Empty;
        public SqlTransaction? Transaction => Command.Transaction;
        private SqlCommand Command { get; set; }

        /// <summary>
        /// Ruft den ConnectionString zur aktuellen Datenbank ab.
        /// </summary>
        public string ConnectionString { get; }

        #region Konstruktoren
        public SqlController(string connectionString)
        {
            ConnectionString = connectionString;
            Command = new SqlCommand
            {
                Connection = new SqlConnection(ConnectionString)
            };

            Command.Connection.Open();
        }
        static SqlController()
        {

            SqlMapper.AddTypeHandler(new GuidTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));

            // INIT Dapper for CompareField
            foreach (Type type in SingletonTypeAttributeCache.CacheAll<CompareFieldAttribute>((att) => att.FieldName))
            {
                SqlMapper.SetTypeMap(type, new CustomPropertyTypeMap(
                    type,
                    (type, columnName) =>
                    {
                        PropertyInfo? prop = SingletonTypeAttributeCache.Get(type, columnName);

                        return prop is null ? type.GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) : prop;

                    }
                ));
            }
        }
        #endregion
        #region SQL-Methoden
        public async Task QueryAsync(string sqlCommand, object? param = null)
        {
            await Command.Connection.QueryAsync(sqlCommand, param, Transaction);
        }

        public Task<T?> GetFirstAsync<T>(string selectCommand, object? param = null)
        {
            Task<T?> result = Command.Connection.QueryFirstOrDefaultAsync<T?>(selectCommand, param, Transaction);
            return result;
        }

        public async Task<List<T>> SelectDataAsync<T>(string selectCommand, object? param = null)
        {
            IEnumerable<T> enumerable = await Command.Connection.QueryAsync<T>(selectCommand, param, Transaction);
            return enumerable.ToList();
        }
        #endregion
        #region Transaction
        /// <summary>
        /// Startet eine neue Transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">Tritt auf, wenn bereits eine Transaction läuft.</exception>
        /// <param name="transactionName">Hilft bei der Identifizierung der Abfragen in Database Workbench.</param>
        public async Task StartTransactionAsync()
        {
            if (Command.Transaction is not null)
            {
                throw new InvalidOperationException($"Es konnte keine Transaction gestartet werden, da bereits eine Transaction läuft");
            }

            Command.Transaction = (SqlTransaction)await Connection.BeginTransactionAsync();
        }
        /// <summary>
        /// Committed alle Änderungen aus der Pipe-Line
        /// <para>
        /// Beim Aufruf der Methode wird die Transaction abgeschlossen und kann nicht mehr verwendet werden.
        /// </para>
        /// </summary>
        public async Task CommitChangesAsync()
        {
            try
            {
                await Command.Transaction.CommitAsync();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                Command.Transaction?.Dispose();
            }
        }
        /// <summary>
        /// Hebt alle ausstehenden Änderungen der Transaction auf.
        /// </summary>
        public async Task RollbackChangesAsync()
        {
            try
            {
                await Command.Transaction.RollbackAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region IDisposable
        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // Dispose schließt die Verbindung automatisch
                    Connection.Dispose();
                    Command.Dispose();
                }

                _disposedValue = true;
            }
        }

        ~SqlController()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
