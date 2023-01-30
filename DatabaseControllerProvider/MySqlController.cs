using Dapper;
using MySql.Data.MySqlClient;

namespace DatabaseControllerProvider
{
    public sealed class MySqlController : IDisposable, IDbController<MySqlConnection, MySqlCommand, MySqlTransaction>
    {
        private bool _disposedValue;
        public MySqlConnection Connection => Command.Connection;
        public MySqlTransaction? Transaction => Command.Transaction;
        public MySqlCommand Command { get; }
        public string CommandText => Command.CommandText ?? String.Empty;

        /// <summary>
        /// Ruft den ConnectionString zur aktuellen Datenbank ab.
        /// </summary>
        public string ConnectionString { get; }

        #region Konstruktoren
        public MySqlController(string connectionString)
        {
            ConnectionString = connectionString;
            Command = new MySqlCommand
            {
                Connection = new MySqlConnection(ConnectionString)
            };

            Command.Connection.Open();
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

            Command.Transaction = (MySqlTransaction)await Connection.BeginTransactionAsync();
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

        ~MySqlController()
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

        public string GetLastIdSql()
        {
            return "SELECT LAST_INSERT_ID();";
        }
    }
}
