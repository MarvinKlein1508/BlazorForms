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
        public async Task CreateAsync(FormRow input, IDbController dbController)
        {
            string sql = @$"INSERT INTO form_rows
(
form_id,
is_active,
sort_order,
)
VALUES
(
@FORM_ID,
@IS_ACTIVE,
@SORT_ORDER
) {dbController.GetLastIdSql()}";

            input.RowId = await dbController.GetFirstAsync<int>(sql, input.GetParameters());
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
            string sql = @"UPDATE form_rows SET
form_id = @FORM_ID,
is_active = @IS_ACTIVE,
SORT_ORDER = @SORT_ORDER
WHERE
row_id = @ROW_ID";

            await dbController.QueryAsync(sql, input.GetParameters());
        }
    }
}
