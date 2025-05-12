using BlazorForms.Domain.Entities;

namespace BlazorForms.Domain.Interfaces;

public interface ILoginService
{
    Task<User?> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
}
