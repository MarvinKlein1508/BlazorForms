using DatabaseControllerProvider;
using FormularPortal.Core.Filters;
using FormularPortal.Core.Models;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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
description,
logo,
image,
login_required,
is_active
)
VALUES
(
@NAME,
@DESCRIPTION,
@LOGO,
@IMAGE,
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

        public async Task<List<Form>> GetAsync(FormFilter filter, IDbController dbController)
        {
            StringBuilder sb = new();
            sb.AppendLine("SELECT * FROM forms WHERE 1 = 1");
            sb.AppendLine(GetFilterWhere(filter));
            sb.AppendLine(@$"  ORDER BY form_id DESC");
            sb.AppendLine(dbController.GetPaginationSyntax(filter.PageNumber, filter.Limit));

            string sql = sb.ToString();

            List<Form> list = await dbController.SelectDataAsync<Form>(sql, GetFilterParameter(filter));

            return list;
        }

        public Dictionary<string, object?> GetFilterParameter(FormFilter filter)
        {
            return new Dictionary<string, object?>
            {
                { "SEARCHPHRASE", $"%{filter.SearchPhrase}%" }
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

            if (filter.ShowOnlyFormsWhichRequireLogin)
            {
                sb.AppendLine(" AND login_required = 1");
            }



            string sql = sb.ToString();
            return sql;
        }

        public async Task<int> GetTotalAsync(FormFilter filter, IDbController dbController)
        {
            StringBuilder sb = new();
            sb.AppendLine("SELECT COUNT(*) FROM forms WHERE 1 = 1");
            sb.AppendLine(GetFilterWhere(filter));

            string sql = sb.ToString();

            int result = await dbController.GetFirstAsync<int>(sql, GetFilterParameter(filter));

            return result;
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
description = @DESCRIPTION,
logo = @LOGO,
image = @IMAGE,
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


            var (newRowIds, newColumnIds, newElementIds, newRuleIds) = GetHashSets(input);
            var (oldRowIds, oldColumnIds, oldElementIds, oldRuleIds) = GetHashSets(oldInputToCompare);

            oldRowIds.ExceptWith(newRowIds);
            oldColumnIds.ExceptWith(newColumnIds);
            oldElementIds.ExceptWith(newElementIds);
            oldRuleIds.ExceptWith(newRuleIds);

            await CleanTableAsync(oldRowIds, "form_rows", "row_id", dbController);
            await CleanTableAsync(oldColumnIds, "form_columns", "column_id", dbController);
            await CleanTableAsync(oldElementIds, "form_elements", "element_id", dbController);
            await CleanTableAsync(oldRuleIds, "form_elements_rules", "rule_id", dbController);
        }

        private static (HashSet<int> rowIds, HashSet<int> columnIds, HashSet<int> elementIds, HashSet<int> ruleIds) GetHashSets(Form input)
        {
            HashSet<int> rowIds = new();
            HashSet<int> columnIds = new();
            HashSet<int> elementIds = new();
            HashSet<int> ruleIds = new();

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

                        if(element is FormTableElement formTableElement)
                        {
                            foreach (var tableElement in formTableElement.Elements)
                            {
                                // Since table elements are within the same table, we can just add it to the same hashset.x
                                elementIds.Add(tableElement.ElementId);
                            }
                        }
                    }
                }
            }

            return (rowIds, columnIds, elementIds, ruleIds);
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
