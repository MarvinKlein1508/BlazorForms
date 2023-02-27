using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormPortal.Core.Services
{
    public class FormEntryService : IModelService<FormEntry, int>
    {
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


        }

        public Task DeleteAsync(FormEntry input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public Task<FormEntry?> GetAsync(int identifier, IDbController dbController)
        {
            throw new NotImplementedException();
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
