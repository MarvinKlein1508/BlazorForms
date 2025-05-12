using BlazorForms.Domain.Entities;
using System.Data;

namespace BlazorForms.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetAsync(int userId, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
}
