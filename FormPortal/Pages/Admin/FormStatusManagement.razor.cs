using FormPortal.Core.Services;
using DatabaseControllerProvider;
using Blazor.Pagination;
using FormPortal.Core.Models;
using FormPortal.Core.Filters;
using Microsoft.AspNetCore.Components.Forms;

namespace FormPortal.Pages.Admin
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
            using IDbController dbController = DbProviderService.GetDbController(AppdatenService.ConnectionString);
            int amount_of_statuses = await Service.GetTotalStatusAmount(dbController);
            if (amount_of_statuses > 1)
            {
                await base.DeleteAsync();
            }
            else
            {
                await JSRuntime.ShowToastAsync(ToastType.error, "Status kann nicht gelöscht werden, da mindestens ein Status vorhanden sein muss.");
            }
        }

    }
}