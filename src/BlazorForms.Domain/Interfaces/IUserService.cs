using BlazorForms.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Domain.Interfaces;

public interface IUserService
{
    Task<User?> GetAsync(int userId, IDbConnection dbConnection, IDbTransaction? dbTransaction = null, CancellationToken cancellationToken = default);
}
