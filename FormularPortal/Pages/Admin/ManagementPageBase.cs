using DatabaseControllerProvider;
using FormularPortal.Core;
using FormularPortal.Core.Models;
using FormularPortal.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace FormularPortal.Pages.Admin
{
    public abstract class ManagementBasePage<T, TService> : ComponentBase where T : class, IDbModel, new() where TService : IModelService<T>
    {
        protected T? Input { get; set; }
        protected T? SelectedForDeletion { get; set; }
        protected EditForm? _form;
#nullable disable
        [Inject] public TService Service { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public DbProviderService DbProviderService { get; set; }
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
            using IDbController dbController = DbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
            await dbController.StartTransactionAsync();

            try
            {
                await Service.DeleteAsync(SelectedForDeletion, dbController);
                await dbController.CommitChangesAsync();
                await JSRuntime.ShowToastAsync(ToastType.success, "Datensatz wurde erfolgreich gelöscht!");
                SelectedForDeletion = null;
            }
            catch (Exception)
            {
                await dbController.RollbackChangesAsync();
                throw;
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
                using IDbController dbController = DbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
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


                await JSRuntime.ShowToastAsync(ToastType.success, "Datensatz wurde erfolgreich gespeichert!");

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
                return $"Datensatz bearbeiten";
            }

            return "Neuer Datensatz";
        }
    }
}
