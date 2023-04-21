using DatabaseControllerProvider;
using FormPortal.Core;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace FormPortal.Pages.Admin
{
    public abstract class ManagementBasePage<T, TService> : ComponentBase where T : class, IDbModel, new() where TService : IModelService<T>
    {
        protected T? Input { get; set; }
        protected T? SelectedForDeletion { get; set; }
        protected EditForm? _form;
#nullable disable
        [Inject] public TService Service { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public IDbProviderService DbProviderService { get; set; }

        [Inject]
        private IStringLocalizer<App> AppLocalizer { get; set; }
#nullable enable

        protected List<T> Data { get; set; } = new();

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }
        protected virtual Task NewAsync()
        {
            Input = new T();
            return Task.CompletedTask;
        }

        protected virtual Task SelectForDeletionAsync(T input)
        {
            SelectedForDeletion = input;
            return Task.CompletedTask;
        }

        protected virtual Task EditAsync(T input)
        {
            Input = input.DeepCopyByExpressionTree();
            return Task.CompletedTask;
        }

        protected virtual async Task DeleteAsync()
        {
            if (SelectedForDeletion is null)
            {
                return;
            }
            using IDbController dbController = DbProviderService.GetDbController(AppdatenService.ConnectionString);
            await dbController.StartTransactionAsync();

            try
            {
                await Service.DeleteAsync(SelectedForDeletion, dbController);
                await dbController.CommitChangesAsync();
                await JSRuntime.ShowToastAsync(ToastType.success, AppLocalizer["DELETE_MESSAGE"]);
                SelectedForDeletion = null;
            }
            catch (Exception ex)
            {
                await dbController.RollbackChangesAsync();
                if (ex.HResult == -2147467259)
                {
                    await JSRuntime.ShowToastAsync(ToastType.error, AppLocalizer["DELETE_ERROR_REFERENCE"]);
                }
                else
                {
                    throw;
                }
            }
        }

        protected virtual async Task SaveAsync()
        {
            if (_form is null || _form.EditContext is null || Input is null)
            {
                return;
            }

            if (_form.EditContext.Validate())
            {
                using IDbController dbController = DbProviderService.GetDbController(AppdatenService.ConnectionString);
                await dbController.StartTransactionAsync();
                try
                {
                    if (Input.Id is 0)
                    {
                        await Service.CreateAsync(Input, dbController);
                    }
                    else
                    {
                        await Service.UpdateAsync(Input, dbController);
                    }

                    await dbController.CommitChangesAsync();
                }
                catch (Exception)
                {
                    await dbController.RollbackChangesAsync();
                    throw;
                }


                await JSRuntime.ShowToastAsync(ToastType.success, AppLocalizer["SAVE_MESSAGE"]);

                Input = null;
            }
        }

        protected virtual string GetModalTitel()
        {
            if (Input is null)
            {
                return string.Empty;
            }

            if (Input.Id > 0)
            {
                return AppLocalizer["MODAL_EDIT_TITLE"];
            }

            return AppLocalizer["MODAL_NEW_TITLE"];
        }
    }
}
