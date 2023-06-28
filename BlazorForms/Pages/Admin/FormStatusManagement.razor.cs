using DbController;
using DbController.MySql;
using BlazorForms.Core.Filters;
using BlazorForms.Core.Models;
using BlazorForms.Core.Services;

namespace BlazorForms.Pages.Admin
{
    public partial class FormStatusManagement
    {
        public FormStatusFilter Filter { get; set; } = new();

        protected override Task NewAsync()
        {
            var newStatus = new FormStatus();

            foreach (var culture in AppdatenService.SupportedCultures)
            {
                newStatus.Description.Add(new FormStatusDescription
                {
                    Code = culture.TwoLetterISOLanguageName
                });
            }

            Input = newStatus;

            return Task.CompletedTask;
        }


        protected override async Task DeleteAsync()
        {
            // Check if more than one status exist
            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
            int amount_of_statuses = await Service.GetTotalStatusAmount(dbController);
            if (amount_of_statuses > 1)
            {
                await base.DeleteAsync();
            }
            else
            {
                await JSRuntime.ShowToastAsync(ToastType.error, "Status kann nicht gel�scht werden, da mindestens ein Status vorhanden sein muss.");
            }
        }

    }
}