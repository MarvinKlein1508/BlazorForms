using DbController;
using DbController.MySql;
using BlazorForms.Core.Constants;
using BlazorForms.Core.Filters;
using BlazorForms.Core.Models;
using BlazorForms.Core.Services;
using Microsoft.AspNetCore.Components;
using BlazorBootstrap;
using BlazorForms.Core;

namespace BlazorForms.Components.Pages.Admin.Forms
{
    public partial class FormListing
    {
        private ConfirmDialog _deleteModal = default!;
        private ConfirmDialog _copyModal = default!;
        public FormFilter Filter { get; set; } = new()
        {
            Limit = Storage.PageLimit
        };

        public List<Form> Data { get; set; } = [];
        [Parameter]
        public int Page { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => TotalItems / Filter.Limit;

        public bool UserCanDeleteForms { get; set; }

        private Task OnPageChangedAsync(int pageNumber)
        {
            navigationManager.NavigateTo($"/Admin/Forms/{pageNumber}");
            return Task.CompletedTask;
        }


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
            using IDbController dbController = new MySqlController();
            TotalItems = await formService.GetTotalAsync(Filter, dbController);
            Data = await formService.GetAsync(Filter, dbController);

        }

        private async Task ShowDeleteModalAsync(Form input)
        {

            var options = new ConfirmDialogOptions
            {
                YesButtonText = AppLocalizer["YES"],
                YesButtonColor = ButtonColor.Success,
                NoButtonText = AppLocalizer["NO"],
                NoButtonColor = ButtonColor.Danger
            };

            var confirmation = await _deleteModal.ShowAsync(
            title: localizer["MODAL_DELETE_TITLE"],
            message1: String.Format(localizer["MODAL_DELETE_TEXT"], input.Name),
            confirmDialogOptions: options);

            if (confirmation)
            {
                using IDbController dbController = new MySqlController();

                await dbController.StartTransactionAsync();

                try
                {
                    await formService.DeleteAsync(input, dbController);
                    await dbController.CommitChangesAsync();
                    await jsRuntime.ShowToastAsync(ToastType.success, localizer["MODAL_DELETE_SUCCESS"]);
                }
                catch (Exception)
                {
                    await dbController.RollbackChangesAsync();
                    throw;
                }

                await LoadAsync();
            }



        }



        private async Task ShowCopyModalAsync(Form input)
        {

            var options = new ConfirmDialogOptions
            {
                YesButtonText = AppLocalizer["YES"],
                YesButtonColor = ButtonColor.Success,
                NoButtonText = AppLocalizer["NO"],
                NoButtonColor = ButtonColor.Danger
            };

            var confirmation = await _deleteModal.ShowAsync(
            title: localizer["MODAL_COPY_TITLE"],
            message1: localizer["MODAL_COPY_DESCRIPTION"],
            confirmDialogOptions: options);

            if (confirmation)
            {
                using IDbController dbController = new MySqlController();

                await dbController.StartTransactionAsync();

                try
                {
                    var form = await formService.GetAsync(input.FormId, dbController);

                    if (form is not null)
                    {
                        await formService.CreateAsync(form, dbController);
                    }

                    await dbController.CommitChangesAsync();
                    await jsRuntime.ShowToastAsync(ToastType.success, localizer["MODAL_COPY_SUCCESS"]);
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
}