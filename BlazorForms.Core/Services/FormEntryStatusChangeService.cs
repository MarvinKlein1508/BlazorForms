using DbController;
using BlazorForms.Core.Models;

namespace BlazorForms.Core.Services
{
    public class FormEntryStatusChangeService : IModelService<FormEntryStatusChange, int>
    {
        public async Task CreateAsync(FormEntryStatusChange input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = $@"INSERT INTO form_entry_history SET
entry_id = @ENTRY_ID,
status_id = @STATUS_ID,
user_id = @USER_ID,
comment = @COMMENT,
date_added = @DATE_ADDED; {dbController.GetLastIdSql()}";

            input.Id = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

            foreach (var item in input.Notifiers)
            {
                item.HistoryId = input.Id;
                sql = @"INSERT INTO form_entry_history_notify
(
history_id,
user_id,
notify
)
VALUES
(
@HISTORY_ID,
@USER_ID,
@NOTIFY
)";

                await dbController.QueryAsync(sql, item.GetParameters(), cancellationToken);
            }
        }

        public async Task DeleteAsync(FormEntryStatusChange input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = "DELETE FROM form_entry_history WHERE history_id = @HISTORY_ID";

            await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
        }

        public async Task<FormEntryStatusChange?> GetAsync(int historyId, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = "SELECT * FROM form_entry_history WHERE history_id = @HISTORY_ID";

            var result = await dbController.GetFirstAsync<FormEntryStatusChange>(sql, new
            {
                HISTORY_ID = historyId
            }, cancellationToken);

            return result;
        }

        public Task UpdateAsync(FormEntryStatusChange input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task ApproveAsync(int entryId, IDbController dbController)
        {
            string sql = "UPDATE form_entries SET approved = 1 WHERE entry_id = @ENTRY_ID";

            await dbController.QueryAsync(sql, new
            {
                ENTRY_ID = entryId
            });
        }

        public async Task<List<FormEntryStatusChange>> GetHistoryAsync(int entryId, IDbController dbController)
        {
            string sql =
"""
SELECT 
    feh.*,
    u.display_name
FROM form_entry_history feh 
LEFT JOIN users u ON (u.user_id = feh.user_id)
WHERE entry_id = @ENTRY_ID 
ORDER BY history_id DESC
""";

            var list = await dbController.SelectDataAsync<FormEntryStatusChange>(sql, new
            {
                ENTRY_ID = entryId
            });

            return list;
        }
    }
}
