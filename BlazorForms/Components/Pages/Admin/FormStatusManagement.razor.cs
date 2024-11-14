using DbController;
using DbController.MySql;
using BlazorForms.Core.Filters;
using BlazorForms.Core.Models;
using BlazorForms.Core.Services;
using BlazorForms.Core;

namespace BlazorForms.Components.Pages.Admin
{
    public partial class FormStatusManagement
    {
        public FormStatusFilter Filter { get; set; } = new();

        protected override Task NewAsync()
        {
            var newStatus = new FormStatus();

            foreach (var culture in Storage.SupportedCultures)
            {
                newStatus.Description.Add(new FormStatusDescription
                {
                    Code = culture.TwoLetterISOLanguageName
                });
            }

            Input = newStatus;

            return Task.CompletedTask;
        }

        protected async Task ShowDeleteModalAsync(FormStatus input)
        {
            // Check if more than one status exist
            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
            int amount_of_statuses = await Service.GetTotalStatusAmount(dbController);
            if (amount_of_statuses > 1)
            {
                await base.ShowDeleteModalAsync(input, localizer["MODAL_DELETE_TITLE"], localizer["MODAL_DELETE_TEXT"], localizer["MODAL_DELETE_SUCCESS"]);
            }
            else
            {
                await JSRuntime.ShowToastAsync(ToastType.error, localizer["ERROR_ONE_STATUS_REQUIRED"]);
            }
        }


    }
}