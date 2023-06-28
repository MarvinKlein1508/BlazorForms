using DbController;
using DbController.MySql;
using BlazorForms.Core.Constants;
using BlazorForms.Core.Filters;
using BlazorForms.Core.Models;
using BlazorForms.Core.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Pages.Admin.Forms
{
    public partial class FormListing
    {
        public FormFilter Filter { get; set; } = new()
        {
            Limit = AppdatenService.PageLimit
        };

        public List<Form> Data { get; set; } = new();
        [Parameter]
        public int Page { get; set; }
        public int TotalItems { get; set; }

        public bool UserCanDeleteForms { get; set; }

        public Form? SelectedForDeletion { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await LoadAsync();
            UserCanDeleteForms = await authService.HasRole(Roles.DELETE_FORMS);

        }
        public async Task LoadAsync(bool navigateToPage1 = false)
        {
            if (navigateToPage1)
            {
                navigationManager.NavigateTo("/Admin/Forms/");
            }

            Filter.PageNumber = Page;
            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
            TotalItems = await formService.GetTotalAsync(Filter, dbController);
            Data = await formService.GetAsync(Filter, dbController);

        }

        private async Task DeleteAsync()
        {
            if (SelectedForDeletion is null)
            {
                return;
            }

            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);

            await dbController.StartTransactionAsync();

            try
            {
                await formService.DeleteAsync(SelectedForDeletion, dbController);
                await dbController.CommitChangesAsync();
                await jsRuntime.ShowToastAsync(ToastType.success, localizer["DELETE_MESSAGE"]);
                SelectedForDeletion = null;
            }
            catch (Exception)
            {
                await dbController.RollbackChangesAsync();
                throw;
            }

            await LoadAsync();

        }

    }
}