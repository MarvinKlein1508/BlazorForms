using DatabaseControllerProvider;
using FormularPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Services
{
    public class FormRowService : IModelService<FormRow, int>
    {
        private readonly FormColumnService _formColumnService;

        public FormRowService(FormColumnService formColumnService)
        {
            _formColumnService = formColumnService;
        }
        public async Task CreateAsync(FormRow input, IDbController dbController)
        {
            input.SetColumnSortOrder();
            string sql = @$"INSERT INTO form_rows
(
form_id,
is_active,
sort_order
)
VALUES
(
@FORM_ID,
@IS_ACTIVE,
@SORT_ORDER
); {dbController.GetLastIdSql()}";

            input.RowId = await dbController.GetFirstAsync<int>(sql, input.GetParameters());

            foreach (var column in input.Columns)
            {
                column.FormId = input.FormId;
                column.RowId = input.RowId;
                await _formColumnService.CreateAsync(column, dbController);
            }
        }

        public async Task DeleteAsync(FormRow input, IDbController dbController)
        {
            string sql = "DELETE FROM form_rows WHERE row_id = @ROW_ID";

            await dbController.QueryAsync(sql, input.GetParameters());
        }

        public Task<FormRow?> GetAsync(int rowId, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(FormRow input, IDbController dbController)
        {
            input.SetColumnSortOrder();
            string sql = @"UPDATE form_rows SET
form_id = @FORM_ID,
is_active = @IS_ACTIVE,
SORT_ORDER = @SORT_ORDER
WHERE
row_id = @ROW_ID";

            await dbController.QueryAsync(sql, input.GetParameters());

            foreach (var column in input.Columns)
            {
                column.FormId = input.FormId;
                column.RowId = input.RowId;
                if (column.ColumnId is 0)
                {
                    await _formColumnService.CreateAsync(column, dbController);
                }
                else
                {
                    await _formColumnService.UpdateAsync(column, dbController);
                }
            }

            // Delete columns which are not part of the row anymore.
            if (input.Columns.Any())
            {
                List<int> columnIds = input.Columns.Select(x => x.ColumnId).ToList();
                sql = $"DELETE FROM form_columns WHERE row_id = @ROW_ID AND column_id NOT IN ({string.Join(",", columnIds)})";
                await dbController.QueryAsync(sql, new
                {
                    ROW_ID = input.RowId,
                });
            }
            else
            {
                sql = "DELETE FROM form_columns WHERE row_id = @ROW_ID";
                await dbController.QueryAsync(sql, new
                {
                    ROW_ID = input.RowId
                });
            }

        }

        public async Task<List<FormRow>> GetRowsForFormAsync(Form form, IDbController dbController)
        {
            string sql = "SELECT * FROM form_rows WHERE form_id = @FORM_ID";

            List<FormRow> rows = await dbController.SelectDataAsync<FormRow>(sql, new
            {
                FORM_ID = form.FormId,
            });

            if (rows.Any())
            {
                List<int> rowIds = rows.Select(x => x.RowId).ToList();

                List<FormColumn> columns = await _formColumnService.GetColumnsForRowsAsync(rowIds, dbController);

                foreach (var row in rows)
                {
                    row.Columns = columns.Where(x => x.RowId == row.RowId).ToList();
                }
            }

            return rows;
        }
    }
}
