using BlazorForms.Domain.Entities;
using BlazorForms.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Application.Services;

public class LoginService(IDbConnectionFactory _dbConnectionFactory) : ILoginService
{
    public async Task<User?> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        using var dbConnection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        User? user = null;
        // Check local accounts first

        throw new NotImplementedException();
    }
}
