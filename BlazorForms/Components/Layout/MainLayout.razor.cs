using BlazorForms.Core.Constants;
using BlazorForms.Core.Models;
using BlazorForms.Core.Services;
using Microsoft.JSInterop;

namespace BlazorForms.Components.Layout
{
    public partial class MainLayout : IDisposable
    {
        public User? User { get; set; }
        private List<Notification> _notifications = [];

        protected override async Task OnInitializedAsync()
        {
            User = await authService.GetUserAsync();

            NotificationService.NotificationsChanged += NotificationService_NotificationsChanged;
        }

        private async Task NotificationService_NotificationsChanged()
        {
            if (User is not null)
            {
                _notifications = notificationService.GetNotifications(User).ToList();
            }

            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            NotificationService.NotificationsChanged -= NotificationService_NotificationsChanged;
        }
    }
}