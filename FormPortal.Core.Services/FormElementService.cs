using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using System.Xml.Linq;

namespace FormPortal.Core.Services
{
    public class FormElementService : IModelService<FormElement, int>
    {
        private readonly RuleService _ruleSetService;
        private readonly CalcRuleService _calcRuleService;

        public FormElementService(RuleService ruleSetService, CalcRuleService calcRuleService)
        {
            _ruleSetService = ruleSetService;
            _calcRuleService = calcRuleService;
        }

        public async Task CreateAsync(FormElement input, IDbController dbController)
        {
            string sql = $@"INSERT INTO form_elements
(
form_id,
row_id,
column_id,
table_parent_element_id,
guid,
name,
type,
is_active,
is_required,
rule_type,
sort_order
)
VALUES
(
@FORM_ID,
@ROW_ID,
@COLUMN_ID,
@TABLE_PARENT_ELEMENT_ID,
@GUID,
@NAME,
@TYPE,
@IS_ACTIVE,
@IS_REQUIRED,
@RULE_TYPE,
@SORT_ORDER
); {dbController.GetLastIdSql()}";

            input.ElementId = await dbController.GetFirstAsync<int>(sql, input.GetParameters());

            // Custom attributes are split in the database in it's own table
            // For example form_elements_{ELEMENT}_attributes
            (string tableName, List<string> fields) = GetCustomAttributeSql(input);

            if (!string.IsNullOrWhiteSpace(tableName))
            {
                sql = $@"INSERT INTO {tableName}
(
{String.Join($",{Environment.NewLine}", fields)}
)
VALUES
(
{String.Join($",{Environment.NewLine}", fields.Select(x => $"@{x}".ToUpper()))}
)";

                await dbController.QueryAsync(sql, input.GetParameters());
            }


            if (input is FormElementWithOptions elementWithOptions)
            {
                await InsertOrUpdateFormElementsOptionsAsync(elementWithOptions, dbController);
            }

            await InsertOrUpdateElementRulesAsync(input, dbController);
            await InsertOrUpdateFormTableElementsAsync(input, dbController);
            await InsertOrUpdateCalcRuleSetsAsync(input, dbController);
        }

        private async Task InsertOrUpdateCalcRuleSetsAsync(FormElement input, IDbController dbController)
        {
            if (input is FormNumberElement formNumberElement)
            {
                formNumberElement.CalcRules.SetSortOrder();
                foreach (var calcRule in formNumberElement.CalcRules)
                {
                    calcRule.ElementId = input.ElementId;
                    if (calcRule.CalcRuleId is 0)
                    {
                        await _calcRuleService.CreateAsync(calcRule, dbController);
                    }
                    else
                    {
                        await _calcRuleService.UpdateAsync(calcRule, dbController);
                    }
                }
            }
        }

        private async Task InsertOrUpdateFormTableElementsAsync(FormElement input, IDbController dbController)
        {
            if (input is FormTableElement tableElement)
            {
                tableElement.Elements.SetSortOrder();
                foreach (var table_element in tableElement.Elements)
                {
                    table_element.FormId = input.FormId;
                    table_element.RowId = input.RowId;
                    table_element.ColumnId = input.ColumnId;
                    table_element.TableParentElementId = input.ElementId;
                    if (table_element.ElementId > 0)
                    {
                        await UpdateAsync(table_element, dbController);
                    }
                    else
                    {
                        await CreateAsync(table_element, dbController);
                    }
                }
            }
        }

        private async Task InsertOrUpdateFormElementsOptionsAsync(FormElementWithOptions input, IDbController dbController)
        {
            string sql = string.Empty;
            foreach (var option in input.Options)
            {
                option.ElementId = input.ElementId;
                if (option.ElementOptionId is 0)
                {
                    sql = $@"INSERT INTO form_elements_options
(
    element_id,
    name
)
VALUES
(
    @ELEMENT_ID,
    @NAME
); {dbController.GetLastIdSql()}";

                    option.ElementOptionId = await dbController.GetFirstAsync<int>(sql, option.GetParameters());

                }
                else
                {
                    sql = @"UPDATE form_elements_options SET
element_id = @ELEMENT_ID,
name = @NAME
WHERE
element_option_id = @ELEMENT_OPTION_ID";

                    await dbController.QueryAsync(sql, option.GetParameters());
                }
            }

        }
        private static (string tableName, List<string> fields) GetCustomAttributeSql(FormElement input)
        {
            string tableName = string.Empty;
            List<string> fields = new();
            fields.Add("element_id");

            if (input is FormCheckboxElement)
            {
                tableName = "form_elements_checkbox_attributes";
            }

            if (input is FormDateElement)
            {
                tableName = "form_elements_date_attributes";
                fields.Add("is_current_date_default");
            }

            if (input is FormFileElement)
            {
                tableName = "form_elements_file_attributes";
            }

            if (input is FormLabelElement)
            {
                tableName = "form_elements_label_attributes";
            }

            if (input is FormNumberElement)
            {
                tableName = "form_elements_number_attributes";
                fields.AddRange(new string[]
                {
                    "decimal_places",
                    "min_value",
                    "max_value"
                });
            }

            if (input is FormRadioElement)
            {
                tableName = "form_elements_radio_attributes";
            }

            if (input is FormSelectElement)
            {
                tableName = "form_elements_select_attributes";
            }

            if (input is FormTableElement)
            {
                tableName = "form_elements_table_attributes";
            }

            if (input is FormTextElement)
            {
                tableName = "form_elements_text_attributes";

            }

            if (input is FormTextareaElement)
            {
                tableName = "form_elements_textarea_attributes";
            }

            return (tableName, fields);
        }

        public async Task DeleteAsync(FormElement input, IDbController dbController)
        {
            string sql = "DELETE FROM form_elements WHERE element_id = @ELEMENT_ID";

            await dbController.QueryAsync(sql, input.GetParameters());
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
table_parent_element_id = @TABLE_PARENT_ELEMENT_ID,
name = @NAME,
is_active = @IS_ACTIVE,
is_required = @IS_REQUIRED,
rule_type = @RULE_TYPE,
sort_order = @SORT_ORDER
WHERE
element_id = @ELEMENT_ID";

            await dbController.QueryAsync(sql, input.GetParameters());

            (string tableName, List<string> fields) = GetCustomAttributeSql(input);

            if (!string.IsNullOrWhiteSpace(tableName))
            {
                // Delete all rows to avoid duplicate key
                sql = $"DELETE FROM {tableName} WHERE element_id = @ELEMENT_ID";
                await dbController.QueryAsync(sql, input.GetParameters());

                sql = $@"INSERT INTO {tableName}
(
{String.Join($",{Environment.NewLine}", fields)}
)
VALUES
(
{String.Join($",{Environment.NewLine}", fields.Select(x => $"@{x}".ToUpper()))}
)";

                await dbController.QueryAsync(sql, input.GetParameters());
            }

            if (input is FormElementWithOptions elementWithOptions)
            {
                await InsertOrUpdateFormElementsOptionsAsync(elementWithOptions, dbController);

                // Delete options which are not part of the object anymore.
                if (elementWithOptions.Options.Any())
                {
                    List<int> optionIds = elementWithOptions.Options.Select(x => x.ElementOptionId).ToList();
                    sql = $"DELETE FROM form_elements_options WHERE element_id = @ELEMENT_ID AND element_option_id NOT IN ({string.Join(",", optionIds)})";

                    await dbController.QueryAsync(sql, new
                    {
                        ELEMENT_ID = elementWithOptions.ElementId
                    });

                }
                else
                {
                    sql = "DELETE FROM form_elements_options WHERE element_id = @ELEMENT_ID";
                    await dbController.QueryAsync(sql, new
                    {
                        ELEMENT_ID = elementWithOptions.ElementId
                    });
                }
            }

            await InsertOrUpdateElementRulesAsync(input, dbController);
            await InsertOrUpdateFormTableElementsAsync(input, dbController);
            await InsertOrUpdateCalcRuleSetsAsync(input, dbController); 
        }

        private async Task InsertOrUpdateElementRulesAsync(FormElement input, IDbController dbController)
        {
            input.Rules.SetSortOrder();
            foreach (var rule in input.Rules)
            {
                rule.FormId = input.FormId;
                rule.RowId = input.RowId;
                rule.ColumnId = input.ColumnId;
                rule.ElementId = input.ElementId;
                if (rule.RuleId is 0)
                {
                    await _ruleSetService.CreateAsync(rule, dbController);
                }
                else
                {
                    await _ruleSetService.UpdateAsync(rule, dbController);
                }
            }

        }


        public Task UpdateAsync(FormElement input, FormElement oldInputToCompare, IDbController dbController)
        {
            throw new NotImplementedException();
        }
    }
}
