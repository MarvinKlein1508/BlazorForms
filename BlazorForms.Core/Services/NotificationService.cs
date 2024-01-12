using BlazorForms.Core.Models;
using DbController.MySql;
using DbController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Core.Services
{
    public class NotificationService : BackgroundService, IModelService<Notification>
    {
        private static List<Notification> _notifications = [];

        public static event Func<Task>? NotificationsChanged;

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

            _notifications.Remove(input);
        }

        public IEnumerable<Notification> GetNotifications(User user)
        {
            return _notifications.Where(x => x.UserId == user.Id);
        }

        public Task UpdateAsync(Notification input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(200));
            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
            while (!stoppingToken.IsCancellationRequested
                 && await timer.WaitForNextTickAsync(stoppingToken))
            {
                string sql =
                    """
                    SELECT 
                        * 
                    FROM 
                        notifications 
                    ORDER BY
                        created DESC
                    """;

                
                _notifications = await dbController.SelectDataAsync<Notification>(sql, cancellationToken: stoppingToken);

                if (NotificationsChanged != null)
                {
                    var handlers = NotificationsChanged.GetInvocationList();

                    foreach (var handler in handlers)
                    {
                        var task = handler.DynamicInvoke() as Task;

                        if (task is not null)
                        {
                            await task;
                        }
                    }
                }
            }
        }
    }
}
