using DbController;
using BlazorForms.Core.Extensions;
using BlazorForms.Core.Models.FormElements;

namespace BlazorForms.Core.Services
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

        public async Task CreateAsync(FormElement input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
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
reset_on_copy,
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
@RESET_ON_COPY,
@RULE_TYPE,
@SORT_ORDER
); {dbController.GetLastIdSql()}";

            input.ElementId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

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

                await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
            }


            if (input is FormElementWithOptions elementWithOptions)
            {
                await InsertOrUpdateFormElementsOptionsAsync(elementWithOptions, true, dbController, cancellationToken);
            }

            await InsertOrUpdateElementRulesAsync(input, true, dbController, cancellationToken);
            await InsertOrUpdateFormTableElementsAsync(input, true, dbController, cancellationToken);
            await InsertOrUpdateCalcRuleSetsAsync(input, true, dbController, cancellationToken);
            await InsertOrUpdateAcceptedFileTypesAsync(input, dbController, cancellationToken);
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
                fields.AddRange(new string[]
                {
                    "is_current_date_default",
                    "min_value",
                    "max_value"
                });
            }

            if (input is FormFileElement)
            {
                tableName = "form_elements_file_attributes";
                fields.AddRange(new string[]
                {
                    "min_size",
                    "max_size",
                    "allow_multiple_files"
                });
            }

            if (input is FormLabelElement)
            {
                tableName = "form_elements_label_attributes";
                fields.AddRange(new string[]
                {
                    "description",
                    "show_on_pdf"
                });
            }

            if (input is FormNumberElement)
            {
                tableName = "form_elements_number_attributes";
                fields.AddRange(new string[]
                {
                    "decimal_places",
                    "min_value",
                    "max_value",
                    "is_summable",
                    "default_value"
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
                fields.AddRange(new string[]
                {
                    "allow_add_rows"
                });
            }

            if (input is FormTextElement)
            {
                tableName = "form_elements_text_attributes";
                fields.AddRange(new string[]
                {
                    "regex_pattern",
                    "regex_validation_message",
                    "min_length",
                    "max_length",
                });

            }

            if (input is FormTextareaElement)
            {
                tableName = "form_elements_textarea_attributes";
                fields.AddRange(new string[]
                {
                    "regex_pattern",
                    "regex_validation_message",
                    "min_length",
                    "max_length",
                });
            }

            return (tableName, fields);
        }

        public async Task DeleteAsync(FormElement input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = "DELETE FROM form_elements WHERE element_id = @ELEMENT_ID";

            await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
        }

        public Task<FormElement?> GetAsync(int identifier, IDbController dbController, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(FormElement input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = @"UPDATE form_elements SET 
form_id = @FORM_ID,
row_id = @ROW_ID,
column_id = @COLUMN_ID,
table_parent_element_id = @TABLE_PARENT_ELEMENT_ID,
name = @NAME,
is_active = @IS_ACTIVE,
is_required = @IS_REQUIRED,
reset_on_copy = @RESET_ON_COPY,
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

                await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
            }

            if (input is FormElementWithOptions elementWithOptions)
            {
                await InsertOrUpdateFormElementsOptionsAsync(elementWithOptions,false, dbController, cancellationToken);

                // Delete options which are not part of the object anymore.
                if (elementWithOptions.Options.Any())
                {
                    List<int> optionIds = elementWithOptions.Options.Select(x => x.ElementOptionId).ToList();
                    sql = $"DELETE FROM form_elements_options WHERE element_id = @ELEMENT_ID AND element_option_id NOT IN ({string.Join(",", optionIds)})";

                    await dbController.QueryAsync(sql, new
                    {
                        ELEMENT_ID = elementWithOptions.ElementId
                    }, cancellationToken);

                }
                else
                {
                    sql = "DELETE FROM form_elements_options WHERE element_id = @ELEMENT_ID";
                    await dbController.QueryAsync(sql, new
                    {
                        ELEMENT_ID = elementWithOptions.ElementId
                    }, cancellationToken);
                }
            }

            await InsertOrUpdateElementRulesAsync(input,false, dbController, cancellationToken);
            await InsertOrUpdateFormTableElementsAsync(input, false, dbController, cancellationToken);
            await InsertOrUpdateCalcRuleSetsAsync(input, false, dbController, cancellationToken);
            await InsertOrUpdateAcceptedFileTypesAsync(input, dbController, cancellationToken);
        }
        private async Task InsertOrUpdateAcceptedFileTypesAsync(FormElement input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (input is FormFileElement fileElement)
            {
                string sql = "DELETE FROM form_elements_file_types WHERE element_id = @ELEMENT_ID";
                await dbController.QueryAsync(sql, new
                {
                    ELEMENT_ID = input.ElementId
                }, cancellationToken);

                foreach (var contentType in fileElement.AcceptedFileTypes)
                {
                    sql = @"INSERT INTO form_elements_file_types
(
element_id,
content_type
)
VALUES
(
@ELEMENT_ID,
@CONTENT_TYPE
)";
                    await dbController.QueryAsync(sql, new
                    {
                        ELEMENT_ID = input.ElementId,
                        CONTENT_TYPE = contentType
                    }, cancellationToken);
                }
            }
        }
        private async Task InsertOrUpdateElementRulesAsync(FormElement input, bool forceCreate, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            input.Rules.SetSortOrder();
            foreach (var rule in input.Rules)
            {
                rule.FormId = input.FormId;
                rule.RowId = input.RowId;
                rule.ColumnId = input.ColumnId;
                rule.ElementId = input.ElementId;
                if (rule.RuleId is 0 || forceCreate)
                {
                    await _ruleSetService.CreateAsync(rule, dbController, cancellationToken);
                }
                else
                {
                    await _ruleSetService.UpdateAsync(rule, dbController, cancellationToken);
                }
            }

        }
        private async Task InsertOrUpdateFormElementsOptionsAsync(FormElementWithOptions input, bool forceCreate, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = string.Empty;
            foreach (var option in input.Options)
            {
                option.ElementId = input.ElementId;
                if (option.ElementOptionId is 0 || forceCreate)
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

                    option.ElementOptionId = await dbController.GetFirstAsync<int>(sql, option.GetParameters(), cancellationToken);

                }
                else
                {
                    sql = @"UPDATE form_elements_options SET
element_id = @ELEMENT_ID,
name = @NAME
WHERE
element_option_id = @ELEMENT_OPTION_ID";

                    await dbController.QueryAsync(sql, option.GetParameters(), cancellationToken);
                }
            }

        }
        private async Task InsertOrUpdateFormTableElementsAsync(FormElement input, bool forceCreate, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (input is FormTableElement tableElement)
            {
                tableElement.Elements.SetSortOrder();
                foreach (var table_element in tableElement.Elements)
                {
                    table_element.FormId = input.FormId;
                    table_element.RowId = input.RowId;
                    table_element.ColumnId = input.ColumnId;
                    table_element.TableParentElementId = input.ElementId;
                    if (table_element.ElementId is 0 || forceCreate)
                    {
                        await CreateAsync(table_element, dbController, cancellationToken);
                    }
                    else
                    {
                        await UpdateAsync(table_element, dbController, cancellationToken);
                    }
                }
            }
        }
        private async Task InsertOrUpdateCalcRuleSetsAsync(FormElement input, bool forceCreate, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (input is FormNumberElement formNumberElement)
            {
                formNumberElement.CalcRules.SetSortOrder();
                foreach (var calcRule in formNumberElement.CalcRules)
                {
                    calcRule.ElementId = input.ElementId;
                    if (calcRule.CalcRuleId is 0 || forceCreate)
                    {
                        await _calcRuleService.CreateAsync(calcRule, dbController, cancellationToken);
                    }
                    else
                    {
                        await _calcRuleService.UpdateAsync(calcRule, dbController, cancellationToken);
                    }
                }
            }
        }
    }
}
