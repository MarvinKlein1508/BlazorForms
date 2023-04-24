using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormPortal.Core.Services
{
    public class FormEntryStatusChangeService : IModelService<FormEntryStatusChange, int>
    {
        public async Task CreateAsync(FormEntryStatusChange input, IDbController dbController)
        {
            string sql = $@"INSERT INTO form_entry_history SET
entry_id = @ENTRY_ID,
status_id = @STATUS_ID,
user_id = @USER_ID,
comment = @COMMENT,
date_added = @DATE_ADDED; {dbController.GetLastIdSql()}";

            input.Id = await dbController.GetFirstAsync<int>(sql, input.GetParameters());  
        }

        public async Task DeleteAsync(FormEntryStatusChange input, IDbController dbController)
        {
            string sql = "DELETE FROM form_entry_history WHERE history_id = @HISTORY_ID";

            await dbController.QueryAsync(sql, input.GetParameters());
        }

        public async Task<FormEntryStatusChange?> GetAsync(int historyId, IDbController dbController)
        {
            string sql = "SELECT * FROM form_entry_history WHERE history_id = @HISTORY_ID";

            var result = await dbController.GetFirstAsync<FormEntryStatusChange>(sql, new
            {
                HISTORY_ID = historyId
            });

            return result;
        }

        public Task UpdateAsync(FormEntryStatusChange input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public async Task ApproveAsync(int entryId, IDbController dbController)
        {

        }
    }
}
