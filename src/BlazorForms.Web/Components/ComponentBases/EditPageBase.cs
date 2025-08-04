using BlazorForms.Application.Common;
using BlazorForms.Application.Common.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Data;

namespace BlazorForms.Web.Components.ComponentBases;

public abstract class EditPageBase<TModel, TService, TIdentifier> : BlazorFormsPageBase
    where TModel : class, IDbModel<TIdentifier>, new()
    where TService : IUpdateOperation<TModel>, IGetOperation<TModel, TIdentifier>, IDeleteOperation<TModel>
{
    protected EditForm? _form;
    protected bool _showOptionsMenu;
    protected bool _showSpinner;
    [SupplyParameterFromQuery(Name = "Id")] public TIdentifier? Identifier { get; set; }

    [Inject] protected TService Service { get; set; } = default!;
    protected TModel? Input { get; private set; }

    protected override async Task OnParametersSetAsync()
    {
        Input = await LoadEditModeAsync();

        if (Input is not null)
        {
            //await CheckActivePageAsync();
        }
    }
    protected async Task SaveAsync()
    {
        if (_form is null || _form.EditContext is null || Input is null)
        {
            return;
        }

        _showSpinner = true;
        if (_form.EditContext.Validate() && await OnValidateAsync())
        {
            using var connection = await DbFactory.CreateConnectionAsync();
            var transaction = connection.BeginTransaction();

            try
            {
                await BeforeSaveAsync(connection, transaction);
                await Service.UpdateAsync(Input, connection, transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                //Log.Logger.Error("#EDIT_PAGE_BASE_1 Error: {ex}", ex);
                transaction.Rollback();
                throw;
            }

            //Log.Information("Saved: {url}; Identifier: {identifier}; User: {user}", NavigationManager.Uri, Input.GetIdentifier(), CurrentUser?.DisplayName ?? "<UNBEKANNT>");
            Storage.UpdateStorage<TModel, TIdentifier>(Input);

            ToastService.ShowSuccess("Datensatz wurde erfolgreich gespeichert");
            await AfterSaveAsync(connection);
            await OnParametersSetAsync();
        }

        _showSpinner = false;
    }
    protected virtual Task BeforeSaveAsync(IDbConnection connection, IDbTransaction transaction) => Task.CompletedTask;
    protected virtual Task AfterSaveAsync(IDbConnection connection) => Task.CompletedTask;
    protected virtual Task BeforeDeleteAsync(IDbConnection connection, IDbTransaction transaction) => Task.CompletedTask;
    protected virtual Task<bool> OnValidateAsync() => Task.FromResult(true);
    protected virtual bool CanSave()
    {
        return true;
    }
    protected virtual async Task<TModel?> LoadEditModeAsync()
    {
        if (Identifier is null)
        {
            return null;
        }

        using var connection = await DbFactory.CreateConnectionAsync();
        return await Service.GetAsync(Identifier, connection);
    }

    protected abstract string GetListUrl();
    protected virtual async Task ShowDeleteModalAsync()
    {
        if (Input is null)
        {
            return;
        }

        var dialog = await DialogService.ShowConfirmationAsync("Möchten Sie diesen Datensatz wirklich löschen?", "Löschen", "Abbrechen", "Datensatz löschen?");
        var result = await dialog.Result;

        if (!result.Cancelled)
        {

            using var connection = await DbFactory.CreateConnectionAsync();
            var transaction = connection.BeginTransaction();
            try
            {
                await BeforeDeleteAsync(connection, transaction);
                await Service.DeleteAsync(Input, connection, transaction);
                transaction.Commit();
                Storage.DeleteFromStorage<TModel, TIdentifier>(Input);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }

            MessageService.ShowMessageBar(options =>
            {
                options.Title = "Datensatz wurde erfolgreich gelöscht!";
                options.Intent = MessageIntent.Success;
                options.Section = FluentConstants.MESSAGEBAR_TOP_SECTION;
                options.Timeout = 4000;
            });

            NavigationManager.NavigateTo(GetListUrl());
        }
    }
    protected string GetTabNavLinkClass(bool active) => active ? "nav-link active" : "nav-link";
    protected string GetTabClass(bool active) => active ? "tab-pane fade active show" : "tab-pane fade";
}

//public abstract class EditPageBase<TModel, TService, TValidator, TIdentifier> : EditPageBase<TModel, TService, TIdentifier>
//    where TModel : class, IDbModel<TIdentifier>, new()
//    where TService : IUpdateOperation<TModel>, IGetOperation<TModel, TIdentifier>, IDeleteOperation<TModel>
//    where TValidator : AbstractValidator<TModel>, new()
//{
//    protected TValidator _validator = new();
//}
