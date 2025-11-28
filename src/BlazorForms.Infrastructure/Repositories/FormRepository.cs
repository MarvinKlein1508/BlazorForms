using System.Data;

namespace BlazorForms.Infrastructure.Repositories;

public sealed class FormRepository(FormRowRepository _formRowRepository) : IModelService<Form, int?>
{
    public Task CreateAsync(Form input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Form input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Form?> GetAsync(int? identifier, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Form input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
