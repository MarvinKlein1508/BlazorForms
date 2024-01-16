using DbController;
using BlazorForms.Core.Constants;
using BlazorForms.Core.Extensions;
using BlazorForms.Core.Filters;
using BlazorForms.Core.Models;
using BlazorForms.Core.Models.FormElements;
using System.Text;

namespace BlazorForms.Core.Services
{
    public class FormService : IModelService<Form, int, FormFilter>
    {
        private readonly FormRowService _formRowService;
        public FormService(FormRowService formRowService)
        {
            _formRowService = formRowService;
        }
        public async Task CreateAsync(Form input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            input.Rows.SetSortOrder();
            string sql =
                $"""
                INSERT INTO forms
                (
                    name,
                    description,
                    logo,
                    image,
                    login_required,
                    is_active,
                    default_status_id,
                    language_id,
                    default_name
                )
                VALUES
                (
                    @NAME,
                    @DESCRIPTION,
                    @LOGO,
                    @IMAGE,
                    @LOGIN_REQUIRED,
                    @IS_ACTIVE,
                    @DEFAULT_STATUS_ID,
                    @LANGUAGE_ID,
                    @DEFAULT_NAME
                ); {dbController.GetLastIdSql()}
                """;

            input.FormId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

            foreach (var row in input.Rows)
            {
                row.FormId = input.FormId;
                await _formRowService.CreateAsync(row, dbController, cancellationToken);
            }

            await CreateOrUpdateFormPermissionsAsync(input, dbController, cancellationToken);
            await CreateOrUpdateFormManagersAsync(input, dbController, cancellationToken);
        }
        public async Task DeleteAsync(Form input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = "DELETE FROM forms WHERE form_id = @FORM_ID";

            await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
        }
        public async Task<Form?> GetAsync(int formId, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = "SELECT * FROM forms WHERE form_id = @FORM_ID";

            Form? form = await dbController.GetFirstAsync<Form>(sql, new
            {
                FORM_ID = formId,
            }, cancellationToken);

            if (form is not null)
            {
                await LoadFormContentAsync(form, 0, dbController, cancellationToken);
            }

            return form;
        }

        public async Task<Form?> GetEntryForm(int formId, int entryId, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = "SELECT * FROM forms WHERE form_id = @FORM_ID";

            Form? form = await dbController.GetFirstAsync<Form>(sql, new
            {
                FORM_ID = formId,
            }, cancellationToken);

            if (form is not null)
            {
                await LoadFormContentAsync(form, entryId, dbController, cancellationToken);
            }

            return form;
        }

        private async Task LoadFormContentAsync(Form form, int entryId, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Load all required data here
            form.AllowedUsersForNewEntries = await GetAllowedUsersForNewFormEntries(form, dbController, cancellationToken);
            form.ManagerUsers = await GetManagersForFormAsync(form, dbController, cancellationToken);
            List<FormRow> rows = await GetRowsAsync(form, dbController, cancellationToken);
            List<FormColumn> columns = await GetColumnsAsync(form, dbController, cancellationToken);
            List<FormElement> elements = await GetElementsAsync(form, entryId, dbController, cancellationToken);
            List<Rule> rules = await GetRulesAsync(form, dbController, cancellationToken);


            List<FormTableElement> tableElements = new();
            // Loop through the data and map everything.
            foreach (var row in rows)
            {
                row.Form = form;
                row.Rules = rules.Where(x => x.RowId == row.RowId && x.ColumnId is null && x.ElementId is null).ToList();
                foreach (var column in columns.Where(x => x.RowId == row.RowId))
                {
                    column.Parent = row;
                    column.Form = form;
                    column.Rules = rules.Where(x => x.RowId == column.RowId && x.ColumnId == column.ColumnId && x.ElementId is null).ToList();
                    foreach (var element in elements.Where(x => x.ColumnId == column.ColumnId))
                    {
                        element.Parent = column;
                        element.Form = form;
                        element.Rules = rules.Where(x => x.RowId == element.RowId && x.ColumnId == element.ColumnId && x.ElementId == element.ElementId).ToList();
                        if (element.TableParentElementId is 0)
                        {
                            column.Elements.Add(element);
                        }
                        else
                        {
                            // Search the parent element and add the current element to it
                            var searchTableElement = elements.FirstOrDefault(x => x.ElementId == element.TableParentElementId) as FormTableElement;
                            if (searchTableElement is not null)
                            {
                                searchTableElement.Elements.Add(element);
                                if (!tableElements.Contains(searchTableElement))
                                {
                                    tableElements.Add(searchTableElement);
                                }
                            }
                        }
                    }

                    row.Columns.Add(column);
                }
            }

            form.Rows = rows;

            if (entryId is > 0)
            {
                List<FormEntryTableElement> tableEntries = await GetTableEntriesAsync(entryId, dbController, cancellationToken);

                // Generate the List of values for this FormTableElement
                foreach (var element in tableElements)
                {
                    var searchEntries = tableEntries.Where(x => x.TableParentElementId == element.ElementId).ToList();

                    int rowAmount = searchEntries.Count / (element.Elements.Count != 0 ? element.Elements.Count : 1);

                    for (int i = 1; i <= rowAmount; i++)
                    {
                        var row = element.NewRow();

                        foreach (var rowElement in row)
                        {
                            FormEntryTableElement? searchValue = searchEntries.FirstOrDefault(x => x.ElementId == rowElement.ElementId && x.TableRowNumber == i);

                            if (searchValue is not null)
                            {
                                rowElement.SetValue(searchValue);
                            }
                        }
                    }
                }
            }

            foreach (var rule in rules)
            {
                rule.Element = elements.FirstOrDefault(x => x.Guid == rule.ElementGuid);
            }



        }
        public Task UpdateAsync(Form input, IDbController dbController, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        /// <summary>
        /// Compares two instances of the same object and updates it in the database when the two objects are different from each other.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="oldInputToCompare"
        /// <param name="dbController"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Form input, Form oldInputToCompare, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            input.Rows.SetSortOrder();
            string sql =
                """
                UPDATE forms SET
                    name = @NAME,
                    description = @DESCRIPTION,
                    logo = @LOGO,
                    image = @IMAGE,
                    login_required = @LOGIN_REQUIRED,
                    is_active = @IS_ACTIVE,
                    default_status_id = @DEFAULT_STATUS_ID,
                    language_id = @LANGUAGE_ID,
                    default_name = @DEFAULT_NAME
                WHERE
                    form_id = @FORM_ID
                """;

            await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);

            foreach (var row in input.Rows)
            {
                row.FormId = input.FormId;
                if (row.RowId is 0)
                {
                    await _formRowService.CreateAsync(row, dbController, cancellationToken);
                }
                else
                {
                    await _formRowService.UpdateAsync(row, dbController, cancellationToken);
                }
            }

            var (newRowIds, newColumnIds, newElementIds, newRuleIds, newCalcRuleIds) = GetHashSets(input);
            var (oldRowIds, oldColumnIds, oldElementIds, oldRuleIds, oldCalcRuleIds) = GetHashSets(oldInputToCompare);

            oldRowIds.ExceptWith(newRowIds);
            oldColumnIds.ExceptWith(newColumnIds);
            oldElementIds.ExceptWith(newElementIds);
            oldRuleIds.ExceptWith(newRuleIds);
            oldCalcRuleIds.ExceptWith(newCalcRuleIds);

            await CleanTableAsync(oldRowIds, "form_rows", "row_id", dbController, cancellationToken);
            await CleanTableAsync(oldColumnIds, "form_columns", "column_id", dbController, cancellationToken);
            await CleanTableAsync(oldElementIds, "form_elements", "element_id", dbController, cancellationToken);
            await CleanTableAsync(oldRuleIds, "form_rules", "rule_id", dbController, cancellationToken);
            await CleanTableAsync(oldCalcRuleIds, "form_elements_number_calc_rules", "calc_rule_id", dbController, cancellationToken);


            await CreateOrUpdateFormPermissionsAsync(input, dbController, cancellationToken);
            await CreateOrUpdateFormManagersAsync(input, dbController, cancellationToken);
        }
        public async Task<List<Form>> GetAsync(FormFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            StringBuilder sb = new();
            sb.AppendLine("SELECT f.* FROM forms f");
            if (filter.UserId > 0)
            {
                sb.AppendLine("LEFT JOIN form_to_user fu ON (f.form_id = fu.form_id)");
            }
            sb.AppendLine("WHERE 1 = 1");
            sb.AppendLine(GetFilterWhere(filter));
            sb.AppendLine(@$"  ORDER BY form_id DESC");
            sb.AppendLine(dbController.GetPaginationSyntax(filter.PageNumber, filter.Limit));

            string sql = sb.ToString();

            List<Form> list = await dbController.SelectDataAsync<Form>(sql, GetFilterParameter(filter), cancellationToken);

            return list;
        }
        public Dictionary<string, object?> GetFilterParameter(FormFilter filter)
        {
            return new Dictionary<string, object?>
            {
                { "SEARCHPHRASE", $"%{filter.SearchPhrase}%" },
                { "USER_ID", filter.UserId },
                { "LANGUAGE_ID", filter.LanguageId }
            };
        }
        public string GetFilterWhere(FormFilter filter)
        {
            StringBuilder sb = new StringBuilder();


            if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
            {
                sb.AppendLine(@" AND 
(
    UPPER(name) LIKE @SEARCHPHRASE
)");

            }

            if (filter.ShowOnlyActiveForms)
            {
                sb.AppendLine(" AND is_active = 1");
            }

            if (filter.HideLoginRequired)
            {
                sb.AppendLine(" AND login_required = 0");
            }

            if (filter.UserId > 0)
            {
                sb.AppendLine(" AND (user_id IS NULL OR user_id = @USER_ID)");
            }

            if (filter.LanguageId > 0)
            {
                sb.AppendLine(" AND language_id = @LANGUAGE_ID");
            }

            string sql = sb.ToString();
            return sql;
        }
        public async Task<int> GetTotalAsync(FormFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            StringBuilder sb = new();
            sb.AppendLine("SELECT COUNT(*) FROM forms f");
            if (filter.UserId > 0)
            {
                sb.AppendLine("LEFT JOIN form_to_user fu ON (f.form_id = fu.form_id)");
            }
            sb.AppendLine("WHERE 1 = 1");
            sb.AppendLine(GetFilterWhere(filter));

            string sql = sb.ToString();

            int result = await dbController.GetFirstAsync<int>(sql, GetFilterParameter(filter), cancellationToken);

            return result;
        }
        private static (HashSet<int> rowIds, HashSet<int> columnIds, HashSet<int> elementIds, HashSet<int> ruleIds, HashSet<int> calcRuleIds) GetHashSets(Form input)
        {
            HashSet<int> rowIds = new();
            HashSet<int> columnIds = new();
            HashSet<int> elementIds = new();
            HashSet<int> ruleIds = new();
            HashSet<int> calcRuleIds = new();

            foreach (var row in input.Rows)
            {
                rowIds.Add(row.RowId);
                foreach (var column in row.Columns)
                {
                    columnIds.Add(column.ColumnId);
                    foreach (var element in column.Elements)
                    {
                        elementIds.Add(element.ElementId);
                        foreach (var rule in element.Rules)
                        {
                            ruleIds.Add(rule.RuleId);
                        }

                        if (element is FormNumberElement numberElement)
                        {
                            foreach (var calcRule in numberElement.CalcRules)
                            {
                                calcRuleIds.Add(calcRule.CalcRuleId);
                            }
                        }

                        if (element is FormTableElement formTableElement)
                        {
                            foreach (var tableElement in formTableElement.Elements)
                            {
                                // Since table elements are within the same table, we can just add it to the same hashset.x
                                elementIds.Add(tableElement.ElementId);


                                if (tableElement is FormNumberElement tableNumberElement)
                                {
                                    foreach (var calcRule in tableNumberElement.CalcRules)
                                    {
                                        calcRuleIds.Add(calcRule.CalcRuleId);
                                    }
                                }
                            }
                        }
                    }

                    foreach (var rule in column.Rules)
                    {
                        ruleIds.Add(rule.RuleId);
                    }
                }

                foreach (var rule in row.Rules)
                {
                    ruleIds.Add(rule.RuleId);
                }
            }

            return (rowIds, columnIds, elementIds, ruleIds, calcRuleIds);
        }

        /// <summary>
        /// Delete all entries for each identifer in the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifiersToDelete"></param>
        /// <param name="tableName"></param>
        /// <param name="identifierColumnName"></param>
        /// <param name="dbController"></param>
        /// <returns></returns>
        private async Task CleanTableAsync<T>(HashSet<T> identifiersToDelete, string tableName, string identifierColumnName, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = string.Empty;
            if (identifiersToDelete.Any())
            {
                sql = $"DELETE FROM {tableName} WHERE {identifierColumnName} IN ({string.Join(",", identifiersToDelete)})";
                await dbController.QueryAsync(sql, null, cancellationToken);
            }
        }
        /// <summary>
        /// Get the base <see cref="FormRow"/> objects for a specific form.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="dbController"></param>
        /// <returns></returns>
        private Task<List<FormRow>> GetRowsAsync(Form form, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = "SELECT * FROM form_rows WHERE form_id = @FORM_ID ORDER BY sort_order";

            return dbController.SelectDataAsync<FormRow>(sql, new
            {
                FORM_ID = form.FormId,
            }, cancellationToken);
        }
        private Task<List<User>> GetAllowedUsersForNewFormEntries(Form form, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql =
                """
                SELECT 
                    u.user_id, 
                    u.username, 
                    u.display_name, 
                    u.email, 
                    u.origin 
                FROM form_to_user fu
                INNER JOIN users u ON (u.user_id = fu.user_id)
                WHERE 
                    fu.form_id = @FORM_ID
                """;

            return dbController.SelectDataAsync<User>(sql, form.GetParameters(), cancellationToken);
        }



        private Task<List<User>> GetManagersForFormAsync(Form form, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql =
                """
                SELECT 
                    u.user_id, 
                    u.username, 
                    u.display_name, 
                    u.email, 
                    u.origin, 
                    fm.receive_email, 
                    fm.can_approve, 
                    fm.status_change_notification_default 
                FROM form_managers fm
                INNER JOIN users u ON (u.user_id = fm.user_id)
                WHERE 
                    fm.form_id = @FORM_ID
                """;

            return dbController.SelectDataAsync<User>(sql, form.GetParameters(), cancellationToken);
        }
        /// <summary>
        /// Get the base <see cref="FormColumn"/> objects for a specific form.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="dbController"></param>
        /// <returns></returns>
        private Task<List<FormColumn>> GetColumnsAsync(Form form, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = $"SELECT * FROM form_columns WHERE form_id = @FORM_ID ORDER BY sort_order";
            return dbController.SelectDataAsync<FormColumn>(sql, new
            {
                FORM_ID = form.FormId,
            }, cancellationToken);
        }
        /// <summary>
        /// Get the base <see cref="FormElement"/> objects for a specific form.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="dbController"></param>
        /// <returns></returns>
        private async Task<List<FormElement>> GetElementsAsync(Form form, int entryId, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            List<FormElement> elements = new();
            foreach (ElementType elementType in Enum.GetValues(typeof(ElementType)))
            {
                // We need to check for each type individually
                string tableName = GetTableForElementType(elementType);

                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    var sqlBuilder = new StringBuilder();
                    sqlBuilder.Append("SELECT fe.*, fea.*");
                    if (entryId > 0)
                    {
                        sqlBuilder.Append(",fee.value_boolean, fee.value_string, fee.value_number, fee.value_date");
                    }

                    sqlBuilder.AppendLine($@" FROM form_elements fe
LEFT JOIN {tableName} fea ON (fea.element_id = fe.element_id)");

                    if (entryId > 0)
                    {
                        sqlBuilder.AppendLine($@"LEFT JOIN form_entries_elements fee ON (fee.element_id = fe.element_id AND fee.form_id = fe.form_id AND fee.entry_id = {entryId})");
                    }

                    sqlBuilder.AppendLine(" WHERE fe.type = @TYPE AND fe.form_id = @FORM_ID");

                    if (entryId > 0)
                    {
                        sqlBuilder.AppendLine(@$" AND 
(
    value_boolean IS NOT NULL 
    OR value_string IS NOT NULL
    OR value_number IS NOT NULL
    OR value_date IS NOT NULL
    OR fe.element_id IN (SELECT DISTINCT fete.element_id FROM form_entries_table_elements fete WHERE entry_id = {entryId})
)");
                    }

                    sqlBuilder.AppendLine(" ORDER BY sort_order");

                    string sql = sqlBuilder.ToString();
                    Dictionary<string, object?> parameters = new Dictionary<string, object?>
                    {
                        { "TYPE", elementType.ToString() },
                        { "FORM_ID", form.FormId }
                    };

                    IEnumerable<FormElement> castedElements = elementType switch
                    {
                        ElementType.Checkbox => await dbController.SelectDataAsync<FormCheckboxElement>(sql, parameters, cancellationToken),
                        ElementType.Date => await dbController.SelectDataAsync<FormDateElement>(sql, parameters, cancellationToken),
                        ElementType.File => await dbController.SelectDataAsync<FormFileElement>(sql, parameters, cancellationToken),
                        ElementType.Label => await dbController.SelectDataAsync<FormLabelElement>(sql, parameters, cancellationToken),
                        ElementType.Number => await dbController.SelectDataAsync<FormNumberElement>(sql, parameters, cancellationToken),
                        ElementType.Radio => await dbController.SelectDataAsync<FormRadioElement>(sql, parameters, cancellationToken),
                        ElementType.Select => await dbController.SelectDataAsync<FormSelectElement>(sql, parameters, cancellationToken),
                        ElementType.Table => await dbController.SelectDataAsync<FormTableElement>(sql, parameters, cancellationToken),
                        ElementType.Text => await dbController.SelectDataAsync<FormTextElement>(sql, parameters, cancellationToken),
                        ElementType.Textarea => await dbController.SelectDataAsync<FormTextareaElement>(sql, parameters, cancellationToken),
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

                            List<FormElementOption> options = await dbController.SelectDataAsync<FormElementOption>(sql, null, cancellationToken);

                            foreach (var item in optionElements)
                            {
                                item.Options = options.Where(x => x.ElementId == item.ElementId).ToList();
                            }
                        }
                    }


                    if (elementType is ElementType.File && castedElements.Any())
                    {
                        IEnumerable<FormFileElement> fileElements = (IEnumerable<FormFileElement>)castedElements;

                        List<int> elementIds = fileElements.Select(x => x.ElementId).ToList();
                        List<FormFileElementFile> files = new();
                        if (entryId > 0)
                        {
                            sql = $"SELECT * FROM form_entries_files WHERE entry_id = @ENTRY_ID AND element_id IN ({string.Join(",", elementIds)})";

                            files = await dbController.SelectDataAsync<FormFileElementFile>(sql, new
                            {
                                ENTRY_ID = entryId
                            }, cancellationToken);
                        }

                        sql = $"SELECT * FROM form_elements_file_types WHERE element_id IN ({string.Join(",", elementIds)})";

                        var acceptedFileTypes = await dbController.SelectDataAsync<(int elementId, string contentType)>(sql, null, cancellationToken);

                        foreach (var item in fileElements)
                        {
                            item.Values = files.Where(x => x.ElementId == item.ElementId && x.EntryId == entryId).ToList();
                            item.AcceptedFileTypes = acceptedFileTypes.Where(x => x.elementId == item.ElementId).Select(x => x.contentType).ToList();
                        }
                    }


                    if (elementType is ElementType.Number)
                    {
                        IEnumerable<FormNumberElement> numberElements = (IEnumerable<FormNumberElement>)castedElements;

                        if (numberElements.Any())
                        {
                            List<int> elementIds = numberElements.Select(x => x.ElementId).ToList();

                            sql = $"SELECT * FROM form_elements_number_calc_rules WHERE element_id IN ({string.Join(",", elementIds)})";

                            List<CalcRule> rules = await dbController.SelectDataAsync<CalcRule>(sql, null, cancellationToken);

                            foreach (var item in numberElements)
                            {
                                item.CalcRules = rules.Where(x => x.ElementId == item.ElementId).ToList();
                            }
                        }
                    }

                    elements.AddRange(castedElements);
                }

            }

            // We need to sort the elements based on their SortOrder
            elements.Sort((x, y) => x.SortOrder.CompareTo(y.SortOrder));

            return elements;
        }

        /// <summary>
        /// Get the base <see cref="Rule"/> objects for a specific form.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="dbController"></param>
        /// <returns></returns>
        private async Task<List<FormEntryTableElement>> GetTableEntriesAsync(int entryId, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (entryId <= 0)
            {
                return new();
            }

            string sql = "SELECT * FROM form_entries_table_elements WHERE entry_id = @ENTRY_ID";

            List<FormEntryTableElement> entries = await dbController.SelectDataAsync<FormEntryTableElement>(sql, new
            {
                ENTRY_ID = entryId
            }, cancellationToken);

            return entries;
        }

        /// <summary>
        /// Get the base <see cref="Rule"/> objects for a specific form.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="dbController"></param>
        /// <returns></returns>
        private async Task<List<Rule>> GetRulesAsync(Form form, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = "SELECT * FROM form_rules WHERE form_id = @FORM_ID order by sort_order";

            List<Rule> rules = await dbController.SelectDataAsync<Rule>(sql, new
            {
                FORM_ID = form.FormId
            }, cancellationToken);

            return rules;
        }
        /// <summary>
        /// Gets the correct element attribute table name for the specified <see cref="ElementType"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        private async Task CreateOrUpdateFormPermissionsAsync(Form input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            string sql = "DELETE FROM form_to_user WHERE form_id = @FORM_ID";
            await dbController.QueryAsync(sql, new
            {
                FORM_ID = input.FormId
            }, cancellationToken);

            foreach (var user in input.AllowedUsersForNewEntries)
            {
                sql =
                    """
                    INSERT INTO form_to_user
                    (
                        form_id,
                        user_id 
                    )
                    VALUES
                    (
                        @FORM_ID,
                        @USER_ID
                    )
                    """;
                    
                await dbController.QueryAsync(sql, new
                {
                    FORM_ID = input.Id,
                    USER_ID = user.Id
                }, cancellationToken);
            }
        }
        private async Task CreateOrUpdateFormManagersAsync(Form input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = "DELETE FROM form_managers WHERE form_id = @FORM_ID";
            await dbController.QueryAsync(sql, new
            {
                FORM_ID = input.FormId
            }, cancellationToken);

            foreach (var user in input.ManagerUsers)
            {
                sql =
                    """
                    INSERT INTO form_managers
                    (
                        form_id,
                        user_id,
                        receive_email,
                        can_approve,
                        status_change_notification_default
                    )
                    VALUES
                    (
                        @FORM_ID,
                        @USER_ID,
                        @RECEIVE_EMAIL,
                        @CAN_APPROVE,
                        @STATUS_CHANGE_NOTIFICATION_DEFAULT
                    )
                    """;
                    
                await dbController.QueryAsync(sql, new
                {
                    FORM_ID = input.Id,
                    USER_ID = user.Id,
                    RECEIVE_EMAIL = user.EmailEnabled,
                    CAN_APPROVE = user.CanApprove,
                    STATUS_CHANGE_NOTIFICATION_DEFAULT = user.StatusChangeNotificationDefault,
                }, cancellationToken);
            }
        }
    }



}
