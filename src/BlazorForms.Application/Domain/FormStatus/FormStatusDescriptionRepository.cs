using Dapper;
using System.Data;

namespace BlazorForms.Application.Domain;

public class FormStatusDescriptionRepository : ICreateOperation<FormStatusDescription>
{
    public Task CreateAsync(FormStatusDescription input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            INSERT INTO form_status_description
            (
                status_id,
                code,
                name,
                description
            )
            VALUES
            (
                @STATUS_ID,
                @CODE,
                @NAME,
                @DESCRIPTION
            )
            """;

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: input.GetParameters(),
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        return connection.ExecuteAsync(command);
    }

    public Task DeleteForFormStatusAsync(int statusId, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM form_status_description WHERE status_id = @STATUS_ID";
        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: new { STATUS_ID = statusId },
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        return connection.ExecuteAsync(command);
    }
}