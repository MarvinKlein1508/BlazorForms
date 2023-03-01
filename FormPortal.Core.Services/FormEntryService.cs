using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormPortal.Core.Services
{
    public class FormEntryService : IModelService<FormEntry, int>
    {
        private readonly FormService _formService;

        public FormEntryService(FormService formService)
        {
            _formService = formService;
        }
        public async Task CreateAsync(FormEntry input, IDbController dbController)
        {
            string sql = $@"INSERT INTO form_entries
(
form_id,
creation_date,
creation_user_id,
last_change,
last_change_user_id
)
VALUES
(
@FORM_ID,
@CREATION_DATE,
@CREATION_USER_ID,
@LAST_CHANGE,
@LAST_CHANGE_USER_ID    
); {dbController.GetLastIdSql()}";

            input.EntryId = await dbController.GetFirstAsync<int>(sql, input.GetParameters());

            foreach (var element in input.Form.GetElements())
            {
                element.EntryId = input.EntryId;
                sql = @"INSERT INTO form_entries_elements
(
entry_id,
element_id,
value_boolean,
value_string,
value_number,
value_date
)
VALUES
(
@ENTRY_ID,
@ELEMENT_ID,
@VALUE_BOOLEAN,
@VALUE_STRING,
@VALUE_NUMBER,
@VALUE_DATE
)";
                await dbController.QueryAsync(sql, element.GetParameters());

                if (element is FormTableElement tableElement)
                {
                    foreach (var row in tableElement.ElementValues)
                    {
                        foreach (var row_element in row)
                        {
                            row_element.EntryId = input.EntryId;
                            sql = $@"INSERT INTO form_entries_table_elements
(
entry_id,
element_id,
value_boolean,
value_string,
value_number,
value_date
)
VALUES
(
@ENTRY_ID,
@ELEMENT_ID,
@VALUE_BOOLEAN,
@VALUE_STRING,
@VALUE_NUMBER,
@VALUE_DATE
); {dbController.GetLastIdSql()}";

                            await dbController.GetFirstAsync<int>(sql, row_element.GetParameters());
                        }
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

                        file.FileId = await dbController.GetFirstAsync<int>(sql, file.GetParameters());
                    }
                }
            }

        }

        public Task DeleteAsync(FormEntry input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public async Task<FormEntry?> GetAsync(int entryId, IDbController dbController)
        {
            string sql = @"SELECT * FROM form_entries WHERE entry_id = @ENTRY_ID";

            FormEntry? entry = await dbController.GetFirstAsync<FormEntry>(sql, new
            {
                ENTRY_ID = entryId
            });

            if (entry is not null)
            {
                entry.Form = await _formService.GetAsync(entry.FormId, dbController) ?? new();
            }

            return entry;
        }

        public Task UpdateAsync(FormEntry input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(FormEntry input, FormEntry oldInputToCompare, IDbController dbController)
        {
            throw new NotImplementedException();
        }
    }
}
