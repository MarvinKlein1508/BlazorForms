using BlazorForms.Core.Models;
using DbController;
using DbController.MySql;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Components.Notifications
{
    public partial class NotificationItem
    {
        [Parameter, EditorRequired]
        public Notification Item { get; set; } = new();
        [Parameter]
        public EventCallback<Notification> OnDelete { get; set; }
        private async Task DeleteAsync()
        {
            using IDbController dbController = new MySqlController();
            await notificationService.DeleteAsync(Item, dbController);
            await OnDelete.InvokeAsync(Item);
        }
    }
}