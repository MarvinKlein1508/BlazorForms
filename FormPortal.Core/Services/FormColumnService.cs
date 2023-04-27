using DbController;
using FormPortal.Core.Extensions;
using FormPortal.Core.Models;

namespace FormPortal.Core.Services
{
    public class FormColumnService : IModelService<FormColumn, int>
    {
        private readonly FormElementService _formElementService;
        private readonly RuleService _ruleService;

        public FormColumnService(FormElementService formElementService, RuleService ruleService)
        {
            _formElementService = formElementService;
            _ruleService = ruleService;
        }



        public async Task CreateAsync(FormColumn input, IDbController dbController)
        {
            input.Elements.SetSortOrder();
            string sql = @$"INSERT INTO form_columns 
(
form_id,
row_id,
is_active,
rule_type,
sort_order
)
VALUES
(
@FORM_ID,
@ROW_ID,
@IS_ACTIVE,
@RULE_TYPE,
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

            foreach (var rule in input.Rules)
            {
                rule.FormId = input.FormId;
                rule.RowId = input.RowId;
                rule.ColumnId = input.ColumnId;
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
            input.Elements.SetSortOrder();
            string sql = @"UPDATE form_columns SET
form_id = @FORM_ID,
row_id = @ROW_ID,
is_active = @IS_ACTIVE,
rule_type = @RULE_TYPE,
sort_order = @SORT_ORDER
WHERE
column_id = @COLUMN_ID";

            await dbController.QueryAsync(sql, input.GetParameters());

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

            foreach (var rule in input.Rules)
            {
                rule.FormId = input.FormId;
                rule.RowId = input.RowId;
                rule.ColumnId = input.ColumnId;
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
