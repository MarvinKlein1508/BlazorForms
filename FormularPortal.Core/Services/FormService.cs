using DatabaseControllerProvider;
using FormularPortal.Core.Filters;
using FormularPortal.Core.Models;

namespace FormularPortal.Core.Services
{
    public class FormService : IModelService<Form, int, FormFilter>
    {
        private readonly FormRowService _formRowService;

        public FormService(FormRowService formRowService)
        {
            _formRowService = formRowService;
        }
        public async Task CreateAsync(Form input, IDbController dbController)
        {
            input.SetRowSortOrder();
            string sql = $@"INSERT INTO forms
(
name,
login_required,
is_active
)
VALUES
(
@NAME,
@LOGIN_REQUIRED,
@IS_ACTIVE
); {dbController.GetLastIdSql()}";

            input.FormId = await dbController.GetFirstAsync<int>(sql, input.GetParameters());

            foreach (var row in input.Rows)
            {
                row.FormId = input.FormId;
                await _formRowService.CreateAsync(row, dbController);
            }
        }

        public async Task DeleteAsync(Form input, IDbController dbController)
        {
            string sql = "DELETE FROM forms WHERE form_id = @FORM_ID";

            await dbController.QueryAsync(sql, input.GetParameters());
        }

        public async Task<Form?> GetAsync(int formId, IDbController dbController)
        {
            string sql = "SELECT * FROM forms WHERE form_id = @FORM_ID";

            Form? form = await dbController.GetFirstAsync<Form>(sql, new
            {
                FORM_ID = formId,
            });

            if (form is not null)
            {
                form.Rows = await _formRowService.GetRowsForFormAsync(form, dbController);
            }

            return form;
        }

        public Task<List<Form>> GetAsync(FormFilter filter, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public object? GetFilterParameter(FormFilter filter)
        {
            throw new NotImplementedException();
        }

        public string GetFilterWhere(FormFilter filter)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalAsync(FormFilter filter, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Form input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Form input, Form oldInputToCompare, IDbController dbController)
        {

            input.SetRowSortOrder();
            string sql = @"UPDATE forms SET
name = @NAME,
login_required = @LOGIN_REQUIRED,
is_active = @IS_ACTIVE
WHERE
form_id = @FORM_ID";

            await dbController.QueryAsync(sql, input.GetParameters());

            foreach (var row in input.Rows)
            {
                row.FormId = input.FormId;
                if (row.RowId is 0)
                {
                    await _formRowService.CreateAsync(row, dbController);
                }
                else
                {
                    await _formRowService.UpdateAsync(row, dbController);
                }
            }


            var (newRowIds, newColumnIds, newElementIds) = GetHashSets(input);
            var (oldRowIds, oldColumnIds, oldElementIds) = GetHashSets(oldInputToCompare);

            oldRowIds.ExceptWith(newRowIds);
            oldColumnIds.ExceptWith(newColumnIds);
            oldElementIds.ExceptWith(newElementIds);

            await CleanTableAsync(oldRowIds, "form_rows", "row_id", dbController);
            await CleanTableAsync(oldColumnIds, "form_columns", "column_id", dbController);
            await CleanTableAsync(oldElementIds, "form_elements", "element_id", dbController);
        }

        private static (HashSet<int> rowIds, HashSet<int> columnIds, HashSet<int> elementIds) GetHashSets(Form input)
        {
            HashSet<int> rowIds = new();
            HashSet<int> columnIds = new();
            HashSet<int> elementIds = new();
            foreach (var row in input.Rows)
            {
                rowIds.Add(row.RowId);
                foreach (var column in row.Columns)
                {
                    columnIds.Add(column.ColumnId);
                    foreach (var element in column.Elements)
                    {
                        elementIds.Add(element.ElementId);
                    }
                }
            }

            return (rowIds, columnIds, elementIds);
        }

        private async Task CleanTableAsync<T>(HashSet<T> identifiersToDelete, string tableName, string identifierColumnName, IDbController dbController)
        {
            string sql = string.Empty;
            if (identifiersToDelete.Any())
            {
                sql = $"DELETE FROM {tableName} WHERE {identifierColumnName} IN ({string.Join(",", identifiersToDelete)})";
                await dbController.QueryAsync(sql);
            }
        }
    }
}
