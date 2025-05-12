using BlazorForms.Domain.Entities;
using BlazorForms.Domain.Interfaces;
using System.Data;

namespace BlazorForms.Application.Services;

public class UserService(IUserRepository _userRepository) : IUserService
{
    public Task<User?> GetAsync(int userId, IDbConnection dbConnection, IDbTransaction? dbTransaction = null, CancellationToken cancellationToken = default)
    {
        return _userRepository.GetAsync(userId, dbConnection, dbTransaction, cancellationToken);
    }
}
