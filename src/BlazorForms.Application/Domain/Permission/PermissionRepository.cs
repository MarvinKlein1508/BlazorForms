using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Application.Domain;
public class PermissionRepository
{
    public static async Task<List<Permission>> GetAllAsync(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql = "SELECT * FROM permissions";

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: null,
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        var results = await connection.QueryAsync<Permission>(command);

        return results.AsList();
    }
}
