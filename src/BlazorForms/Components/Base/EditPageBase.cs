using BlazorForms.Domain;
using BlazorForms.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Components;

public abstract class EditPageBase<TModel, TRepository, TIdentifier> : BlazorFormsPageBase
    where TModel : class, IDbModel<TIdentifier>, new()
    where TRepository : IUpdateOperation<TModel>, IGetOperation<TModel, TIdentifier>
{
    [SupplyParameterFromQuery(Name = "Id")] public TIdentifier? Identifier { get; set; }
    [Inject] protected TRepository Service { get; set; } = default!;
    protected TModel? Input { get; private set; }
    protected TModel? StartCopy { get; private set; }
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Input = await LoadEditModeAsync();

        if (Input is not null)
        {
            StartCopy = Input.DeepClone();
        }
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

    protected async Task SaveAsync()
    {
        if (Input is null)
        {
            return;
        }

        await DialogService.ShowOverlayAsync(options =>
        {
            options.Text = "Speichern...";
        });

        using var connection = await DbFactory.CreateConnectionAsync();
        var transaction = connection.BeginTransaction();
        try
        {
            //await BeforeSaveAsync(connection, transaction);
            await Service.UpdateAsync(Input, connection, transaction);
            transaction.Commit();
        }
        catch (Exception)
        {
            //Log.Logger.Error("#EDIT_PAGE_BASE_1 Error: {ex}", ex);
            transaction.Rollback();
            throw;
        }

        //Log.Information("Saved: {url}; Identifier: {identifier}; User: {user}", NavigationManager.Uri, Input.GetIdentifier(), CurrentUser?.DisplayName ?? "<UNBEKANNT>");

        await NotificationService.ShowSuccessToastAsync(title: "Datensatz wurde erfolgreich gespeichert");

        //await AfterSaveAsync(connection);
        await OnParametersSetAsync();

        await DialogService.HideOverlayAsync();
    }
}
