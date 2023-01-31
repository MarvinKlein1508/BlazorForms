using DatabaseControllerProvider;
using FormularPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormularPortal.Core.Services
{
    public class FormElementService : IModelService<FormElement, int>
    {
        public async Task CreateAsync(FormElement input, IDbController dbController)
        {
            string sql = $@"INSERT INTO form_elements
(
form_id,
row_id,
column_id,
name,
is_active,
is_required,
sort_order
)
VALUES
(
form_id = @FORM_ID,
row_id = @ROW_ID,
column_id = @COLUMN_ID,
name = @NAME,
is_active = @IS_ACTIVE,
is_required = @IS_REQUIRED,
sort_order = @SORT_ORDER
) {dbController.GetLastIdSql()}";

            input.ElementId = await dbController.GetFirstAsync<int>(sql, new
            {
                FORM_ID = input.FormId,
                ROW_ID = input.RowId,
                COLUMN_ID = input.ColumnId,
                NAME = input.Name,
                IS_ACTIVE = input.IsActive,
                IS_REQUIRED = input.IsRequired,
                SORT_ORDER = input.SortOrder
            });

            // TODO: Insert custom attribute data
        }

        public async Task DeleteAsync(FormElement input, IDbController dbController)
        {
            string sql = "DELETE FROM form_elements WHERE element_id = @ELEMENT_ID";

            await dbController.QueryAsync(sql, new
            {
                ELEMENT_ID = input.ElementId
            });
        }

        public Task<FormElement?> GetAsync(int identifier, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(FormElement input, IDbController dbController)
        {
            string sql = @"UPDATE form_elements SET 
form_id = @FORM_ID,
row_id = @ROW_ID,
column_id = @COLUMN_ID,
name = @NAME,
is_active = @IS_ACTIVE,
is_required = @IS_REQUIRED,
sort_order = @SORT_ORDER
WHERE
element_id = @ELEMENT_ID";

            await dbController.QueryAsync(sql, new
            {
                FORM_ID = input.FormId,
                ROW_ID = input.RowId,
                COLUMN_ID = input.ColumnId,
                NAME = input.Name,
                IS_ACTIVE = input.IsActive,
                IS_REQUIRED = input.IsRequired,
                SORT_ORDER = input.SortOrder,
                ELEMENT_ID = input.ElementId
            });

            // TODO: Insert custom attribute data
        }
    }
}
