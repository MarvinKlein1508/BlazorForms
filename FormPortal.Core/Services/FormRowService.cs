using DbController;
using FormPortal.Core.Extensions;
using FormPortal.Core.Models;

namespace FormPortal.Core.Services
{
    public class FormRowService : IModelService<FormRow, int>
    {
        private readonly FormColumnService _formColumnService;
        private readonly RuleService _ruleService;

        public FormRowService(FormColumnService formColumnService, RuleService ruleService)
        {
            _formColumnService = formColumnService;
            _ruleService = ruleService;
        }
        public async Task CreateAsync(FormRow input, IDbController dbController)
        {
            input.Columns.SetSortOrder();
            string sql = @$"INSERT INTO form_rows
(
form_id,
is_active,
rule_type,
sort_order
)
VALUES
(
@FORM_ID,
@IS_ACTIVE,
@RULE_TYPE,
@SORT_ORDER
); {dbController.GetLastIdSql()}";

            input.RowId = await dbController.GetFirstAsync<int>(sql, input.GetParameters());

            foreach (var column in input.Columns)
            {
                column.FormId = input.FormId;
                column.RowId = input.RowId;
                await _formColumnService.CreateAsync(column, dbController);
            }

            foreach (var rule in input.Rules)
            {
                rule.FormId = input.FormId;
                rule.RowId = input.RowId;
                if (rule.RuleId is 0)
                {
                    await _ruleService.CreateAsync(rule, dbController);
                }
                else
                {
                    await _ruleService.UpdateAsync(rule, dbController);
                }
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
            input.Columns.SetSortOrder();
            string sql = @"UPDATE form_rows SET
form_id = @FORM_ID,
is_active = @IS_ACTIVE,
rule_type = @RULE_TYPE,
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

            foreach (var rule in input.Rules)
            {
                rule.FormId = input.FormId;
                rule.RowId = input.RowId;
                if (rule.RuleId is 0)
                {
                    await _ruleService.CreateAsync(rule, dbController);
                }
                else
                {
                    await _ruleService.UpdateAsync(rule, dbController);
                }
            }
        }
    }
}
