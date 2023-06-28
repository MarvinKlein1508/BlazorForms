using DbController;
using DbController.MySql;
using BlazorForms.Core;
using BlazorForms.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using BlazorBootstrap;
using BlazorForms.Core.Models;

namespace BlazorForms.Pages.Admin
{
    public abstract class ManagementBasePage<T, TService> : ComponentBase where T : class, IDbModel, new() where TService : IModelService<T>
    {
        protected T? Input { get; set; }
        
        protected EditForm? _form;
        protected ConfirmDialog _deleteModal = default;
#nullable disable
        [Inject] public TService Service { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        [Inject] protected IStringLocalizer<App> AppLocalizer { get; set; }
#nullable enable

        protected List<T> Data { get; set; } = new();

        protected override Task OnInitializedAsync()
        {
            Data = AppdatenService.GetList<T>();
            return base.OnInitializedAsync();
        }
        protected virtual Task NewAsync()
        {
            Input = new T();
            return Task.CompletedTask;
        }

       

        protected virtual Task EditAsync(T input)
        {
            Input = input.DeepCopyByExpressionTree();
            return Task.CompletedTask;
        }

        protected virtual async Task SaveAsync()
        {
            if (_form is null || _form.EditContext is null || Input is null)
            {
                return;
            }

            if (_form.EditContext.Validate())
            {
                using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
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
                    AppdatenService.UpdateRecord(Input);
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

        protected async Task<bool> ShowDeleteModalAsync(T input, string modalTitle, string modalMessage, string deleteSuccessMessage)
        {
            var options = new ConfirmDialogOptions
            {
                YesButtonText = AppLocalizer["YES"],
                YesButtonColor = ButtonColor.Success,
                NoButtonText = AppLocalizer["NO"],
                NoButtonColor = ButtonColor.Danger
            };

            var confirmation = await _deleteModal.ShowAsync(
            title: modalTitle,
            message1: modalMessage,
            confirmDialogOptions: options);

            if (confirmation)
            {
                using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
                await dbController.StartTransactionAsync();

                try
                {
                    await Service.DeleteAsync(input, dbController);
                    await dbController.CommitChangesAsync();
                    AppdatenService.DeleteRecord(input);
                    await JSRuntime.ShowToastAsync(ToastType.success, deleteSuccessMessage);
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

                return true;
            }

            return false;

        }
    }
}
