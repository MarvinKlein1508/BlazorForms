using BlazorForms.Core.Models;
using DbController;
using DbController.MySql;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Components.Notifications
{
    public partial class NotificationCenter
    {

        [Parameter, EditorRequired]
        public List<Notification> Notifications { get; set; } = [];

        private async Task DeleteAsync(Notification notification)
        {
            using IDbController dbController = new MySqlController();
            await notificationService.DeleteAsync(notification, dbController);
        }

    }
}