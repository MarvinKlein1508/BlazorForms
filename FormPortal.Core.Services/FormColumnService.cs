﻿using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Services
{
    public class FormColumnService : IModelService<FormColumn, int>
    {
        private readonly FormElementService _formElementService;

        public FormColumnService(FormElementService formElementService)
        {
            _formElementService = formElementService;
        }
        public async Task CreateAsync(FormColumn input, IDbController dbController)
        {
            input.SetElementSortOrder();
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
); {dbController.GetLastIdSql()}";

            input.ColumnId = await dbController.GetFirstAsync<int>(sql, input.GetParameters());

            foreach (var element in input.Elements)
            {
                element.FormId = input.FormId;
                element.RowId = input.RowId;
                element.ColumnId = input.ColumnId;
                await _formElementService.CreateAsync(element, dbController);
            }
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
            input.SetElementSortOrder();
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

            foreach (var element in input.Elements)
            {
                element.FormId = input.FormId;
                element.RowId = input.RowId;
                element.ColumnId = input.ColumnId;
                if (element.ElementId is 0)
                {
                    await _formElementService.CreateAsync(element, dbController);
                }
                else
                {
                    await _formElementService.UpdateAsync(element, dbController);
                }
            }
        }

        public async Task<List<FormColumn>> GetColumnsForRowsAsync(List<int> rowIds, IDbController dbController)
        {
            if (!rowIds.Any())
            {
                return new();
            }

            string sql = $"SELECT * FROM form_columns WHERE row_id IN ({string.Join(",", rowIds)}) ORDER BY sort_order";

            List<FormColumn> columns = await dbController.SelectDataAsync<FormColumn>(sql);

            if (columns.Any())
            {
                // Load elements
                List<int> columnIds = columns.Select(x => x.ColumnId).ToList();

                List<FormElement> elements = await _formElementService.GetElementsForColumns(columnIds, dbController);

                foreach (var column in columns)
                {
                    foreach (var element in elements.Where(x => x.ColumnId == column.ColumnId))
                    {
                        element.Parent = column;
                        column.Elements.Add(element);
                    }
                }
            }

            return columns;
        }

        public Task UpdateAsync(FormColumn input, FormColumn oldInputToCompare, IDbController dbController)
        {
            throw new NotImplementedException();
        }
    }
}