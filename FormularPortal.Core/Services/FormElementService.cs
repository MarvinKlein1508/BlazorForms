using DatabaseControllerProvider;
using FormularPortal.Core.Models;

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
@NAME,
@TYPE,
@IS_ACTIVE,
@IS_REQUIRED,
@SORT_ORDER
); {dbController.GetLastIdSql()}";

            input.ElementId = await dbController.GetFirstAsync<int>(sql, input.GetParameters());

            // TODO: Insert custom attribute data
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


            // TODO: Insert Custom attribute options
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
name = @NAME,
is_active = @IS_ACTIVE,
is_required = @IS_REQUIRED,
sort_order = @SORT_ORDER
WHERE
element_id = @ELEMENT_ID";

            await dbController.QueryAsync(sql, input.GetParameters());

            // TODO: Insert custom attribute data

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
        }

        public async Task<List<FormElement>> GetElementsForColumns(List<int> columnIds, IDbController dbController)
        {
            if (!columnIds.Any())
            {
                return new();
            }

            List<FormElement> elements = new List<FormElement>();

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

                    if (elementType is ElementType.Select or ElementType.Radio)
                    {
                        // TODO: Load all options for this elements
                    }

                    elements.AddRange(castedElements);
                }
            }

            // We'nn need to sort the elements based on their SortOrder
            elements.Sort((x, y) => x.SortOrder.CompareTo(y.SortOrder));


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
    }
}
