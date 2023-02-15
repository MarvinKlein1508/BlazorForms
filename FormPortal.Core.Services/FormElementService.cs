using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using System.Xml.Linq;

namespace FormPortal.Core.Services
{
    public class FormElementService : IModelService<FormElement, int>
    {
        private readonly RuleSetService _ruleSetService;
        private readonly CalcRuleService _calcRuleService;

        public FormElementService(RuleSetService ruleSetService, CalcRuleService calcRuleService)
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
                formNumberElement.SortCalcRuleSets();
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
                tableElement.SortTableElements();
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
            input.SetRuleSortOrder();
            foreach (var rule in input.Rules)
            {
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

        public async Task<List<FormElement>> GetElementsForColumns(Form form, List<int> columnIds, IDbController dbController)
        {
            if (!columnIds.Any())
            {
                return new();
            }

            List<FormElement> elements = new();
            List<FormElement> table_elements = new();


            foreach (ElementType elementType in Enum.GetValues(typeof(ElementType)))
            {
                // We need to check for each type individually
                string tableName = GetTableForElementType(elementType);

                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    string sql = @$"SELECT * FROM form_elements fe
LEFT JOIN {tableName} fea ON (fea.element_id = fe.element_id)
WHERE fe.type = @TYPE AND fe.column_id IN ({string.Join(",", columnIds)})";

                    Dictionary<string, object?> parameters = new Dictionary<string, object?>
                    {
                        { "TYPE", elementType }
                    };

                    IEnumerable<FormElement> castedElements = elementType switch
                    {
                        ElementType.Checkbox => await dbController.SelectDataAsync<FormCheckboxElement>(sql, parameters),
                        ElementType.Date => await dbController.SelectDataAsync<FormDateElement>(sql, parameters),
                        ElementType.File => await dbController.SelectDataAsync<FormFileElement>(sql, parameters),
                        ElementType.Label => await dbController.SelectDataAsync<FormLabelElement>(sql, parameters),
                        ElementType.Number => await dbController.SelectDataAsync<FormNumberElement>(sql, parameters),
                        ElementType.Radio => await dbController.SelectDataAsync<FormRadioElement>(sql, parameters),
                        ElementType.Select => await dbController.SelectDataAsync<FormSelectElement>(sql, parameters),
                        ElementType.Table => await dbController.SelectDataAsync<FormTableElement>(sql, parameters),
                        ElementType.Text => await dbController.SelectDataAsync<FormTextElement>(sql, parameters),
                        ElementType.Textarea => await dbController.SelectDataAsync<FormTextareaElement>(sql, parameters),
                        _ => Array.Empty<FormElement>(),
                    };


                    // Load all available options for types that have a list of available options.
                    if (elementType is ElementType.Select or ElementType.Radio)
                    {
                        IEnumerable<FormElementWithOptions> optionElements = (IEnumerable<FormElementWithOptions>)castedElements;

                        if (optionElements.Any())
                        {
                            List<int> elementIds = optionElements.Select(x => x.ElementId).ToList();

                            sql = $"SELECT * FROM form_elements_options WHERE element_id IN ({string.Join(",", elementIds)})";

                            List<FormElementOption> options = await dbController.SelectDataAsync<FormElementOption>(sql);

                            foreach (var item in optionElements)
                            {
                                item.Options = options.Where(x => x.ElementId == item.ElementId).ToList();
                            }
                        }

                    }

                    foreach (var element in castedElements)
                    {
                        if (element.TableParentElementId > 0)
                        {
                            table_elements.Add(element);
                        }
                        else
                        {
                            elements.Add(element);
                        }
                    }
                }
            }

            // We need to sort the elements based on their SortOrder
            elements.Sort((x, y) => x.SortOrder.CompareTo(y.SortOrder));

            List<int> elementIdsForRules = elements.Select(x => x.ElementId).ToList();
            elementIdsForRules.AddRange(table_elements.Select(x => x.ElementId));

            List<Rule> rules = await _ruleSetService.GetRulesForElements(elementIdsForRules, dbController);
            List<CalcRule> calcRules = await _calcRuleService.GetCalcRulesForElements(elementIdsForRules, dbController);

            foreach (var element in elements)
            {
                element.Form = form;
                foreach (var rule in rules.Where(x => x.ElementId == element.ElementId))
                {
                    element.Rules.Add(rule);
                    rule.Parent = element;
                    rule.Element = elements.FirstOrDefault(x => x.Guid == rule.ElementGuid);
                }

                if (element is FormTableElement formTableElement)
                {
                    formTableElement.Elements = table_elements.Where(x => x.TableParentElementId == element.ElementId).OrderBy(x => x.SortOrder).ToList();
                }

                if(element is FormNumberElement formNumberElement)
                {
                    foreach (var rule in calcRules.Where(x => x.ElementId == element.ElementId))
                    {
                        formNumberElement.CalcRules.Add(rule);
                    }
                }
            }

            foreach (var table_element in table_elements)
            {
                table_element.Form = form;
                if (table_element is FormNumberElement formNumberElement)
                {
                    foreach (var rule in calcRules.Where(x => x.ElementId == table_element.ElementId))
                    {
                        formNumberElement.CalcRules.Add(rule);
                    }
                }
            }
            return elements;
        }

        private string GetTableForElementType(ElementType type) => type switch
        {
            ElementType.Checkbox => "form_elements_checkbox_attributes",
            ElementType.Date => "form_elements_date_attributes",
            ElementType.File => "form_elements_file_attributes",
            ElementType.Label => "form_elements_label_attributes",
            ElementType.Number => "form_elements_number_attributes",
            ElementType.Radio => "form_elements_radio_attributes",
            ElementType.Select => "form_elements_select_attributes",
            ElementType.Table => "form_elements_table_attributes",
            ElementType.Text => "form_elements_text_attributes",
            ElementType.Textarea => "form_elements_textarea_attributes",
            _ => string.Empty,
        };

        public Task UpdateAsync(FormElement input, FormElement oldInputToCompare, IDbController dbController)
        {
            throw new NotImplementedException();
        }
    }
}
