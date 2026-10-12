using BlazorForms.Domain;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Web.Components;

public abstract class CreatePageBase<TModel, TRepository, TIdentifier> : BlazorFormsPageBase
    where TModel : class, IDbModel<TIdentifier>, new()
    where TRepository : ICreateOperation<TModel>
{
    [Inject] public TRepository Service { get; set; } = default!;

    public TModel? Input { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Input = new TModel();
        await InitializeModelAsync();
    }

    protected virtual Task InitializeModelAsync() => Task.CompletedTask;

    protected async Task SaveAsync()
    {
        if (Input is null)
        {
            return;
        }

        using var connection = await DbFactory.CreateConnectionAsync();
        var transaction = connection.BeginTransaction();

        try
        {
            await Service.CreateAsync(Input, connection, transaction);
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}
