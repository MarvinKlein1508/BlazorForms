using BlazorForms.Core.Models;
using DbController;

namespace BlazorForms.Core.Services
{
    public class NotificationService : IModelService<Notification>
    {
        public async Task CreateAsync(Notification input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string sql =
$"""
INSERT INTO notifications
(
    created,
    user_id,
    icon,
    title,
    details,
    href,
    is_read,
    read_timestamp
)
VALUES
(
    @CREATED,
    @USER_ID,
    @ICON,
    @TITLE,
    @DETAILS,
    @HREF,
    @IS_READ,
    @READ_TIMESTAMP
); {dbController.GetLastIdSql()}
""";
            input.Id = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
        }

        public async Task DeleteAsync(Notification input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string sql =
"""
DELETE FROM 
    notifications 
WHERE
    notification_id = @NOTIFICATION_ID 
""";
            await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
        }

        public Task UpdateAsync(Notification input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
