using DatabaseControllerProvider;
using FormularPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Services
{
    public class FormColumnService : IModelService<FormColumn, int>
    {
        public async Task CreateAsync(FormColumn input, IDbController dbController)
        {
            string sql = @$"INSERT INTO form_columns 
(
form_id,
row_id,
is_active,
sort_order
)
VALUES
(
@FORM_ID,
@ROW_ID,
@IS_ACTIVE,
@SORT_ORDER
) {dbController.GetLastIdSql()}";

            input.ColumnId = await dbController.GetFirstAsync<int>(sql, new
            {
                FORM_ID = input.FormId,
                ROW_ID = input.RowId,
                IS_ACTIVE = input.IsActive,
                SORT_ORDER = input.SortOrder
            });
        }

        public async Task DeleteAsync(FormColumn input, IDbController dbController)
        {
            string sql = "DELETE FROM form_columns WHERE column_id = @COLUMN_ID";

            await dbController.QueryAsync(sql, new
            {
                COLUMN_ID = input.ColumnId
            });
        }

        public Task<FormColumn?> GetAsync(int identifier, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(FormColumn input, IDbController dbController)
        {
            string sql = @"UPDATE form_columns SET
form_id = @FORM_ID,
row_id = @ROW_ID,
is_active = @IS_ACTIVE,
sort_order = @SORT_ORDER
WHERE
column_id = @COLUMN_ID";

            await dbController.QueryAsync(sql, new
            {
                FORM_ID = input.FormId,
                ROW_ID = input.RowId,
                IS_ACTIVE = input.IsActive,
                SORT_ORDER = input.SortOrder,
                COLUMN_ID = input.ColumnId
            });
        }
    }
}
