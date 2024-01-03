using BlazorForms.Core.Models;

namespace BlazorForms.Components.Notifications
{
    public partial class NotificationCenter : IDisposable
    {
        public User? User { get; set; }

        protected override async Task OnInitializedAsync()
        {
            User = await authService.GetUserAsync();
            notificationCenterService.User = User;

            notificationCenterService.NotificationsChanged += NotificationCenterService_NotificationsChanged;
        }

        private async void NotificationCenterService_NotificationsChanged(object? sender, EventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            notificationCenterService.NotificationsChanged -= NotificationCenterService_NotificationsChanged;
            notificationCenterService.Dispose();
        }
    }
}