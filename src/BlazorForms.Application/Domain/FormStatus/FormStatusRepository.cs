using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Application.Domain;
public class FormStatusRepository(FormStatusDescriptionRepository _formStatusDescriptionRepository) : IModelService<FormStatus, int?>
{
    public async Task CreateAsync(FormStatus input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            INSERT INTO form_status
            (
                requires_approval,
                is_completed,
                sort_order
            )
            VALUES
            (
                @REQUIRES_APPROVAL,
                @IS_COMPLETED,
                @SORT_ORDER
            ) RETURNING status_id
            """;

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: input.GetParameters(),
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        input.StatusId = await connection.ExecuteScalarAsync<int>(command);

        foreach (var description in input.Descriptions)
        {
            description.StatusId = input.StatusId;
            await _formStatusDescriptionRepository.CreateAsync(description, connection, transaction, cancellationToken);
        }
    }

    public async Task DeleteAsync(FormStatus input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        await _formStatusDescriptionRepository.DeleteForFormStatusAsync(input.StatusId, connection, transaction, cancellationToken);
    }

    public Task<FormStatus?> GetAsync(int? identifier, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(FormStatus input, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE form_status SET
                requires_approval = @REQUIRES_APPROVAL,
                is_completed = @IS_COMPLETED,
                sort_order = @SORT_ORDER
            WHERE 
                status_id = @STATUS_ID
            """;

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: input.GetParameters(),
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        await connection.ExecuteAsync(command);

        await _formStatusDescriptionRepository.DeleteForFormStatusAsync(input.StatusId, connection, transaction, cancellationToken);

        foreach (var description in input.Descriptions)
        {
            description.StatusId = input.StatusId;
            await _formStatusDescriptionRepository.CreateAsync(description, connection, transaction, cancellationToken);
        }
    }
}
