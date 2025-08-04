using Dapper;
using System.Data;
using System.Text;

namespace BlazorForms.Application.Domain;
public class FormStatusRepository(FormStatusDescriptionRepository _formStatusDescriptionRepository) : IModelService<FormStatus, int?, FormStatusFilter>
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

    public async Task<FormStatus?> GetAsync(int? identifier, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        if (identifier == null)
        {
            return null;
        }

        string sql =
            """
            SELECT fs.*, fsd.*
                    FROM form_status fs
                    LEFT JOIN form_status_description fsd ON fs.status_id = fsd.status_id
                    WHERE fs.status_id = @StatusId
            """;
        FormStatus? formStatus = null;

        var result = await connection.QueryAsync<FormStatus, FormStatusDescription, FormStatus>(
            sql,
            (status, description) =>
            {
                formStatus ??= status;

                if (description != null)
                {
                    formStatus.Descriptions.Add(description);
                }

                return formStatus;
            },
            new { StatusId = identifier },
            transaction: transaction,
            splitOn: "status_id"
        );

        return formStatus;
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

    public static async Task<List<FormStatus>> GetAllAsync(IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            """
            SELECT fs.*, fsd.*
            FROM form_status fs
            LEFT JOIN form_status_description fsd ON fs.status_id = fsd.status_id
            ORDER BY fs.sort_order
            """;

        var statusDictionary = new Dictionary<int, FormStatus>();

        var result = await connection.QueryAsync<FormStatus, FormStatusDescription, FormStatus>(sql,
            (status, description) =>
            {
                if (!statusDictionary.TryGetValue(status.StatusId, out var formStatus))
                {
                    formStatus = status;
                    statusDictionary.Add(formStatus.StatusId, formStatus);
                }

                if (description != null)
                {
                    formStatus.Descriptions.Add(description);
                }

                return formStatus;
            },
            transaction: transaction,
            splitOn: "status_id"
        );

        var list = statusDictionary.Values.AsList();

        return list;
    }

    public async Task<PagedResponse<FormStatus>> GetAsync(FormStatusFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        string sql =
            $"""
            SELECT fs.*, fsd.*
            FROM form_status fs
            LEFT JOIN form_status_description fsd ON fs.status_id = fsd.status_id
            WHERE 1 = 1 {GetFilterWhere(filter)}
            ORDER BY fs.sort_order
            """;

        var statusDictionary = new Dictionary<int, FormStatus>();

        var result = await connection.QueryAsync<FormStatus, FormStatusDescription, FormStatus>(sql,
            (status, description) =>
            {
                if (!statusDictionary.TryGetValue(status.StatusId, out var formStatus))
                {
                    formStatus = status;
                    statusDictionary.Add(formStatus.StatusId, formStatus);
                }

                if (description != null)
                {
                    formStatus.Descriptions.Add(description);
                }

                return formStatus;
            },
            transaction: transaction,
            param: filter.GetParameters(),
            splitOn: "status_id"
        );

        var list = statusDictionary.Values.AsList();

        int total = await GetTotalAsync(filter, connection, transaction, cancellationToken);

        var response = new PagedResponse<FormStatus>
        {
            Items = list,
            Page = filter.PageNumber,
            PageSize = filter.Limit,
            Total = total
        };

        return response;
    }

    public Task<int> GetTotalAsync(FormStatusFilter filter, IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*) 
            FROM form_status fs
            LEFT JOIN form_status_description fsd ON (fs.status_id = fsd.status_id)
            WHERE 1 = 1 {GetFilterWhere(filter)}
            """;

        var command = new CommandDefinition
        (
            commandText: sql,
            commandType: CommandType.Text,
            parameters: filter.GetParameters(),
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        return connection.ExecuteScalarAsync<int>(command);
    }

    public string GetFilterWhere(FormStatusFilter filter)
    {
        StringBuilder sb = new();
        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(" AND UPPER(name) LIKE @SEARCH_PHRASE");
        }

        sb.AppendLine(" AND fsd.code = @LANGUAGE_CODE");

        string sql = sb.ToString();
        return sql;
    }
}
