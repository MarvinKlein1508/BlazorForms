using BlazorForms.Core.Filters;
using BlazorForms.Core.Models;
using System.Text;

namespace BlazorForms.Core.Services
{
    public class FormStatusService : IModelService<FormStatus, int, FormStatusFilter>
    {
        public async Task CreateAsync(FormStatus input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql =
                $"""
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
                ); {dbController.GetLastIdSql()}
                """;

            input.StatusId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

            await CreateOrUpdateDescriptionsAsync(input, dbController, cancellationToken);
        }

        public Task DeleteAsync(FormStatus input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = "DELETE FROM form_status WHERE status_id = @STATUS_ID";

            return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
        }

        public async Task<FormStatus?> GetAsync(int statusId, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = "SELECT * FROM form_status WHERE status_id = @STATUS_ID";

            var result = await dbController.GetFirstAsync<FormStatus>(sql, new
            {
                STATUS_ID = statusId
            }, cancellationToken);

            if (result is not null)
            {
                sql = "SELECT * FROM form_status_description WHERE status_id = @STATUS_ID";
                result.Description = await dbController.SelectDataAsync<FormStatusDescription>(sql, new
                {
                    STATUS_ID = statusId
                }, cancellationToken);
            }

            return result;
        }

        public async Task<List<FormStatus>> GetAsync(FormStatusFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql =
                $"""
                SELECT 
                    fs.* 
                FROM form_status fs
                LEFT JOIN form_status_description fsd ON (fs.status_id = fsd.status_id)
                WHERE 1 = 1 {GetFilterWhere(filter)}
                ORDER BY 
                    sort_order ASC
                {dbController.GetPaginationSyntax(filter.PageNumber, filter.Limit)}
                """;

            List<FormStatus> list = await dbController.SelectDataAsync<FormStatus>(sql, GetFilterParameter(filter), cancellationToken);

            if (list.Count != 0)
            {
                IEnumerable<int> statusIds = list.Select(x => x.StatusId);
                sql = $"SELECT * FROM form_status_description WHERE status_id IN ({string.Join(",", statusIds)})";
                List<FormStatusDescription> descriptions = await dbController.SelectDataAsync<FormStatusDescription>(sql, null, cancellationToken);

                foreach (var status in list)
                {
                    status.Description = descriptions.Where(x => x.StatusId == status.StatusId).ToList();
                }

            }

            return list;
        }

        public Dictionary<string, object?> GetFilterParameter(FormStatusFilter filter)
        {
            return new Dictionary<string, object?>
            {
                { "SEARCH_PHRASE", $"%{filter.SearchPhrase}%" },
                { "CULTURE_CODE", filter.Culture.TwoLetterISOLanguageName }
            };
        }

        public string GetFilterWhere(FormStatusFilter filter)
        {
            StringBuilder sb = new();
            if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
            {
                sb.AppendLine(" AND name LIKE @SEARCH_PHRASE");
            }

            sb.AppendLine(" AND fsd.code = @CULTURE_CODE");

            string sql = sb.ToString();
            return sql;
        }

        public Task<int> GetTotalAsync(FormStatusFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
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

            return dbController.GetFirstAsync<int>(sql, GetFilterParameter(filter), cancellationToken);
        }

        public async Task UpdateAsync(FormStatus input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql =
                """
                UPDATE form_status SET
                    requires_approval = @REQUIRES_APPROVAL,
                    is_completed = @IS_COMPLETED,
                    sort_order = @SORT_ORDER
                WHERE 
                    status_id = @STATUS_ID
                """;

            await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);

            await CreateOrUpdateDescriptionsAsync(input, dbController, cancellationToken);
        }

        private async Task CreateOrUpdateDescriptionsAsync(FormStatus input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Delete all entries for this status id first
            string sql = "DELETE FROM form_status_description WHERE status_id = @STATUS_ID";
            await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);

            foreach (var parameters in input.GetLocalizedParameters())
            {
                sql =
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

                await dbController.QueryAsync(sql, parameters, cancellationToken);
            }
        }

        public static async Task<List<FormStatus>> GetAllAsync(IDbController dbController)
        {
            string sql = "SELECT * FROM form_status";

            var statuses = await dbController.SelectDataAsync<FormStatus>(sql);

            sql = "SELECT * FROM form_status_description";

            var descriptions = await dbController.SelectDataAsync<FormStatusDescription>(sql);

            foreach (var status in statuses)
            {
                status.Description = descriptions.Where(x => x.StatusId == status.StatusId).ToList();
            }

            return statuses;
        }

        public Task<int> GetTotalStatusAmount(IDbController dbController)
        {
            string sql = "SELECT COUNT(*) FROM form_status";

            return dbController.GetFirstAsync<int>(sql);
        }
    }
}
