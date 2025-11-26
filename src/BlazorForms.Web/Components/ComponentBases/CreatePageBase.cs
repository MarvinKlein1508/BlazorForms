using BlazorForms.Domain.Interfaces;
using BlazorForms.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Data;

namespace BlazorForms.Web.Components.ComponentBases;

public abstract class CreatePageBase<TModel, TService, TIdentifier> : BlazorFormsPageBase
    where TModel : class, IDbModel<TIdentifier>, new()
    where TService : ICreateOperation<TModel>
{
    protected EditForm? _form;
    protected bool _showSpinner;

    [SupplyParameterFromQuery(Name = "CopyId")]
    public TIdentifier? CopyId { get; set; }

    [Inject] public TService Service { get; set; } = default!;
    public TModel? Input { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (CopyId is not null)
        {
            Input = await InitializeCopyModel();
        }
        else
        {
            Input = new TModel();
        }
    }

    protected virtual Task<TModel?> InitializeCopyModel()
    {
        // Copying objects isn't supported by default
        throw new NotImplementedException();
    }

    protected async Task SaveAsync()
    {
        if (_form is null || _form.EditContext is null || Input is null)
        {
            return;
        }

        if (!_form.EditContext.Validate() && await OnValidateAsync())
        {
            return;
        }

        _showSpinner = true;
        using var connection = await DbFactory.CreateConnectionAsync();
        var transaction = connection.BeginTransaction();

        try
        {
            await BeforeSaveAsync(connection, transaction);
            await Service.CreateAsync(Input, connection, transaction);
            transaction.Commit();
        }
        catch (Exception ex)
        {
            //Log.Logger.Error("#CREATE_PAGE_BASE_1 Error: {ex}", ex);
            transaction.Rollback();
            throw;
        }

        Storage.UpdateStorage<TModel, TIdentifier>(Input);
        //Log.Information("Saved: {url}; Identifier: {identifier}; User: {user}", NavigationManager.Uri, CopyId?.ToString() ?? "<NEU>", CurrentUser?.DisplayName ?? "<UNBEKANNT>");
        //ToastService.ShowSuccess("Datensatz wurde erfolgreich gespeichert");

        await AfterSaveAsync(connection);

        NavigationManager.NavigateTo(GetEntityRedirectUrl());
        _showSpinner = false;
    }

    protected virtual Task BeforeSaveAsync(IDbConnection connection, IDbTransaction transaction) => Task.CompletedTask;
    protected virtual Task AfterSaveAsync(IDbConnection connection) => Task.CompletedTask;
    protected virtual Task<bool> OnValidateAsync() => Task.FromResult(true);
    protected abstract string GetEntityRedirectUrl();
    protected string GetTabNavLinkClass(bool active) => active ? "nav-link active" : "nav-link";
    protected string GetTabClass(bool active) => active ? "tab-pane fade active show" : "tab-pane fade";
}

//public abstract class CreatePageBase<TModel, TService, TValidator, TIdentifier> : CreatePageBase<TModel, TService, TIdentifier>
//    where TModel : class, IDbModel<TIdentifier>, new()
//    where TService : ICreateOperation<TModel>
//    where TValidator : AbstractValidator<TModel>, new()
//{
//    protected TValidator _validator = new();
//}
