using BlazorForms.Core.Models;
using DbController.MySql;
using DbController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Core.Services
{
    public class NotificationCenterService : IDisposable
    {
        public int CountNotifications => Notifications.Count;

        public List<Notification> Notifications { get; set; } = [];

        private System.Timers.Timer _timer;

        public event EventHandler? NotificationsChanged;
        public User? User { get; set; }


        public NotificationCenterService()
        {
            _timer = new();
            _timer.Interval = 1000;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = true;
            _timer.AutoReset = false;
            _timer.Start();

        }

        private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            await CheckAsync();
        }

        private async Task CheckAsync()
        {
            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);

            if (User is null)
            {
                Notifications = [];
            }
            else
            {
                string sql =
                    """
                    SELECT 
                        * 
                    FROM 
                        notifications 
                    WHERE
                        user_id = @USER_ID
                    ORDER BY
                        created DESC
                    """;

                Notifications = await dbController.SelectDataAsync<Notification>(sql, new
                {
                    USER_ID = User.Id
                });
            }

            NotificationsChanged?.Invoke(this, EventArgs.Empty);
            _timer?.Start();
        }

        public void Dispose()
        {
            _timer.Elapsed -= Timer_Elapsed;
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
