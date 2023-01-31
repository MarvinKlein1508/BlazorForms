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

        public async Task UpdateAsync(Form input, IDbController dbController)
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

            // TODO: Delete rows which are not part of the object anymore.
        }
    }
}
