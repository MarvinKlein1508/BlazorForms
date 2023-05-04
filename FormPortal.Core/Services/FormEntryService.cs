using DbController;
using FormPortal.Core.Filters;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using System.Text;

namespace FormPortal.Core.Services
{
    public class FormEntryService : IModelService<FormEntry, int>
    {
        private readonly FormService _formService;

        public FormEntryService(FormService formService)
        {
            _formService = formService;
        }
        public async Task CreateAsync(FormEntry input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = $@"INSERT INTO form_entries
(
form_id,
name,
creation_date,
creation_user_id,
last_change,
last_change_user_id,
status_id,
approved
)
VALUES
(
@FORM_ID,
@NAME,
@CREATION_DATE,
@CREATION_USER_ID,
@LAST_CHANGE,
@LAST_CHANGE_USER_ID,
@STATUS_ID,
@APPROVED
); {dbController.GetLastIdSql()}";

            input.EntryId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

            await CreateElementsAsync(input, dbController, cancellationToken);
        }
        private async Task CreateElementsAsync(FormEntry input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = string.Empty;
            foreach (var element in input.Form.GetElements())
            {
                element.EntryId = input.EntryId;
                sql = @"INSERT INTO form_entries_elements
(
entry_id,
form_id,
element_id,
value_boolean,
value_string,
value_number,
value_date
)
VALUES
(
@ENTRY_ID,
@FORM_ID,
@ELEMENT_ID,
@VALUE_BOOLEAN,
@VALUE_STRING,
@VALUE_NUMBER,
@VALUE_DATE
)";
                await dbController.QueryAsync(sql, element.GetParameters(), cancellationToken);

                if (element is FormTableElement tableElement)
                {
                    int rowNumber = 1;
                    foreach (var row in tableElement.ElementValues)
                    {
                        foreach (var row_element in row)
                        {
                            row_element.EntryId = input.EntryId;
                            sql = $@"INSERT INTO form_entries_table_elements
(
table_row_number,
table_parent_element_id,
entry_id,
element_id,
value_boolean,
value_string,
value_number,
value_date
)
VALUES
(
@TABLE_ROW_NUMBER,
@TABLE_PARENT_ELEMENT_ID,
@ENTRY_ID,
@ELEMENT_ID,
@VALUE_BOOLEAN,
@VALUE_STRING,
@VALUE_NUMBER,
@VALUE_DATE
); {dbController.GetLastIdSql()}";

                            var parameters = row_element.GetParameters();
                            parameters.Add("TABLE_ROW_NUMBER", rowNumber);

                            await dbController.GetFirstAsync<int>(sql, parameters, cancellationToken);
                        }

                        rowNumber++;
                    }
                }

                if (element is FormFileElement fileElement)
                {
                    foreach (var file in fileElement.Values)
                    {
                        file.EntryId = input.EntryId;
                        file.ElementId = element.ElementId;
                        sql = @$"INSERT INTO form_entries_files
(
entry_id,
element_id,
data,
content_type,
filename
)
VALUES
(
@ENTRY_ID,
@ELEMENT_ID,
@DATA,
@CONTENT_TYPE,
@FILENAME
); {dbController.GetLastIdSql()}";

                        file.FileId = await dbController.GetFirstAsync<int>(sql, file.GetParameters(), cancellationToken);
                    }
                }
            }
        }

        public Task DeleteAsync(FormEntry input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public async Task DeleteAsync(EntryListItem input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = "DELETE FROM form_entries WHERE entry_id = @ENTRY_ID";
            await dbController.QueryAsync(sql, new
            {
                ENTRY_ID = input.EntryId
            }, cancellationToken);
        }
        public async Task<FormEntry?> GetAsync(int entryId, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = @"SELECT * FROM form_entries WHERE entry_id = @ENTRY_ID";

            FormEntry? entry = await dbController.GetFirstAsync<FormEntry>(sql, new
            {
                ENTRY_ID = entryId
            }, cancellationToken);

            if (entry is not null)
            {
                entry.Form = await _formService.GetEntryForm(entry.FormId, entryId, dbController, cancellationToken) ?? new();
                entry.Form.EntryMode = true;

            }

            return entry;
        }

        public async Task UpdateAsync(FormEntry input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = @"UPDATE form_entries SET
name = @NAME,
last_change = @LAST_CHANGE,
last_change_user_id = @LAST_CHANGE_USER_ID
WHERE
entry_id = @ENTRY_ID";

            await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);

            // Remove all entry_elements and insert everything again
            // Removing in the base table results in all other being deleted automatically
            var deleteSqls = new List<string>
            {
                "DELETE FROM form_entries_elements WHERE entry_id = @ENTRY_ID",
                "DELETE FROM form_entries_files WHERE entry_id = @ENTRY_ID",
                "DELETE FROM form_entries_table_elements WHERE entry_id = @ENTRY_ID"
            };

            foreach (var deleteSql in deleteSqls)
            {
                await dbController.QueryAsync(deleteSql, input.GetParameters(), cancellationToken);
            }

            await CreateElementsAsync(input, dbController, cancellationToken);
        }
        public async Task<List<EntryListItem>> GetAsync(FormEntryFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine($@"SELECT DISTINCT fe.*, 
COALESCE(u1.display_name, '') AS username_creator, 
COALESCE(u2.display_name, '') AS username_last_change,
f.name AS form_name
FROM form_entries fe
LEFT JOIN forms f ON f.form_id = fe.form_id
LEFT JOIN form_entries_elements fee ON (fee.form_id = fe.form_id AND fee.entry_id = fe.entry_id)
LEFT JOIN users u1 ON (u1.user_id = fe.creation_user_id)
LEFT JOIN users u2 ON (u2.user_id = fe.last_change_user_id)");

            if (filter.SearchAssigned)
            {
                sqlBuilder.AppendLine("INNER JOIN form_managers fm ON (fm.form_id = f.form_id)");
            }
            sqlBuilder.AppendLine("WHERE 1 = 1");
            sqlBuilder.AppendLine(GetFilterWhere(filter));
            sqlBuilder.Append(" ORDER BY entry_id DESC ");
            sqlBuilder.AppendLine(dbController.GetPaginationSyntax(filter.PageNumber, filter.Limit));

            string sql = sqlBuilder.ToString();

            List<EntryListItem> entries = await dbController.SelectDataAsync<EntryListItem>(sql, GetFilterParameter(filter), cancellationToken);

            // We need the managers to be able to check for delete permissions
            if (entries.Any())
            {
                List<int> formIds = entries.Select(x => x.FormId).Distinct().ToList();

                sql = $@"SELECT * FROM form_managers WHERE form_id IN ({string.Join(",", formIds)})";

                List<FormManagerMapping> mappings = await dbController.SelectDataAsync<FormManagerMapping>(sql, null, cancellationToken);

                if (mappings.Any())
                {
                    foreach (var entry in entries)
                    {
                        entry.ManagerIds = mappings.Where(x => x.FormId == entry.FormId).Select(x => x.UserId).ToList();
                    }
                }

            }

            return entries;
        }

        public async Task<int> GetTotalAsync(FormEntryFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine(@$"SELECT COUNT(DISTINCT(fe.entry_id))
FROM form_entries fe
LEFT JOIN forms f ON f.form_id = fe.form_id
LEFT JOIN form_entries_elements fee ON (fee.form_id = fe.form_id AND fee.entry_id = fe.entry_id)
LEFT JOIN users u1 ON (u1.user_id = fe.creation_user_id)
LEFT JOIN users u2 ON (u2.user_id = fe.last_change_user_id)");

            if (filter.SearchAssigned)
            {
                sqlBuilder.AppendLine("INNER JOIN form_managers fm ON (fm.form_id = f.form_id)");
            }

            sqlBuilder.AppendLine("WHERE 1 = 1");

            sqlBuilder.AppendLine(GetFilterWhere(filter));

            string sql = sqlBuilder.ToString();

            int result = await dbController.GetFirstAsync<int>(sql, GetFilterParameter(filter), cancellationToken);

            return result;
        }

        public string GetFilterWhere(FormEntryFilter filter)
        {
            StringBuilder sqlBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
            {
                sqlBuilder.AppendLine(@" AND 
(
    f.name LIKE @SEARCHPHRASE
OR fe.name LIKE @SEARCHPHRASE
OR u1.display_name LIKE @SEARCHPHRASE
OR u2.display_name LIKE @SEARCHPHRASE
)");
            }

            if (filter.StatusId > 0)
            {
                sqlBuilder.AppendLine(" AND status_id = @STATUS_ID");
            }

            if (filter.UserId > 0 && !filter.SearchAssigned)
            {
                sqlBuilder.AppendLine(@" AND creation_user_id = @USERID");
            }

            if (filter.SearchAssigned)
            {
                sqlBuilder.AppendLine(@" AND fm.user_id = @USERID");
            }

            string sql = sqlBuilder.ToString();
            return sql;
        }

        public Dictionary<string, object?> GetFilterParameter(FormEntryFilter filter)
        {
            return new Dictionary<string, object?>
            {
                { "SEARCHPHRASE", $"%{filter.SearchPhrase}%" },
                { "USERID", filter.UserId },
                { "STATUS_ID", filter.StatusId }
            };
        }
    }
}
