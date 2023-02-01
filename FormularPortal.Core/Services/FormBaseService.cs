using DatabaseControllerProvider;
using FormularPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Services
{
    public abstract class FormBaseService
    {
        /// <summary>
        /// Deletes certain elements from the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keepList">A list of <see cref="IDbModel"/> </param>
        /// <param name="tableName">The name of the table from which the records should be deleted</param>
        /// <param name="identifier">The name of the unique identifier column.</param>
        /// <param name="identifierValue">The value of the unique identifier column.</param>
        /// <param name="columnNameIds">The name of the column to check the keepList against</param>
        /// <param name="dbController"></param>
        /// <returns></returns>
        protected virtual async Task CleanElementsAsync<T>(IEnumerable<IDbModel> keepList, string tableName, string identifier, T identifierValue, string columnNameIds, IDbController dbController)
        {
            string sql = string.Empty;
            Dictionary<string, object?> parameters = new Dictionary<string, object?>
            {
                { identifier, identifierValue }
            };

            if (keepList.Any())
            {
                List<int> keepIds = keepList.Select(x => x.Id).ToList();
                sql = $"DELETE FROM {tableName} WHERE {identifier} = @{identifier.ToUpper()} AND {columnNameIds} NOT IN ({string.Join(",", keepIds)})";
                await dbController.QueryAsync(sql, parameters);
            }
            else
            {
                sql = $"DELETE FROM {tableName} WHERE {identifier} = @{identifier.ToUpper()}";
                await dbController.QueryAsync(sql, parameters);
            }
        }

    }
}
