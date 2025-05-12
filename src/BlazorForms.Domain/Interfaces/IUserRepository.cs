using BlazorForms.Domain.Entities;
using System.Data;

namespace BlazorForms.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetAsync(int userId, IDbConnection connection, IDbTransaction? dbTransaction = null, CancellationToken cancellationToken = default);
}
