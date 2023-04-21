using DatabaseControllerProvider;
using FormPortal.Core.Filters;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;
using System.Text;

namespace FormPortal.Core.Services
{
    public class FormStatusService : IModelService<FormStatus, int, FormStatusFilter>
    {
        public async Task CreateAsync(FormStatus input, IDbController dbController)
        {
            string sql = $@"INSERT INTO form_status
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
); {dbController.GetLastIdSql()}";

            input.Id = await dbController.GetFirstAsync<int>(sql, input.GetParameters());

            await CreateOrUpdateDescriptionsAsync(input, dbController);
        }

        public async Task DeleteAsync(FormStatus input, IDbController dbController)
        {
            string sql = "DELETE FROM form_status WHERE status_id = @STATUS_ID";

            await dbController.QueryAsync(sql, input.GetParameters());
        }

        public async Task<FormStatus?> GetAsync(int statusId, IDbController dbController)
        {
            string sql = "SELECT * FROM form_status WHERE status_id = @STATUS_ID";

            var result = await dbController.GetFirstAsync<FormStatus>(sql, new
            {
                STATUS_ID = statusId
            });

            if (result is not null)
            {
                sql = "SELECT * FROM form_status_description WHERE status_id = @STATUS_ID";
                result.Description = await dbController.SelectDataAsync<FormStatusDescription>(sql, new
                {
                    STATUS_ID = statusId
                });
            }

            return result;
        }

        public async Task<List<FormStatus>> GetAsync(FormStatusFilter filter, IDbController dbController)
        {
            StringBuilder sqlBuilder = new();
            sqlBuilder.AppendLine("SELECT fs.* FROM form_status fs");
            sqlBuilder.AppendLine("LEFT JOIN form_status_description fsd ON (fs.status_id = fsd.status_id)");
            sqlBuilder.AppendLine("WHERE 1 = 1");
            sqlBuilder.AppendLine(GetFilterWhere(filter));
            sqlBuilder.AppendLine(@$"  ORDER BY sort_order ASC");
            sqlBuilder.AppendLine(dbController.GetPaginationSyntax(filter.PageNumber, filter.Limit));

            // Zum Debuggen schreiben wir den Wert einmal als Variabel
            string sql = sqlBuilder.ToString();

            List<FormStatus> list = await dbController.SelectDataAsync<FormStatus>(sql, GetFilterParameter(filter));


            if (list.Any())
            {
                IEnumerable<int> statusIds = list.Select(x => x.Id);
                sql = $"SELECT * FROM form_status_description WHERE status_id IN ({string.Join(",", statusIds)})";
                List<FormStatusDescription> descriptions = await dbController.SelectDataAsync<FormStatusDescription>(sql);

                foreach (var status in list)
                {
                    status.Description = descriptions.Where(x => x.StatusId == status.Id).ToList();

                    // TODO: Add missing languages
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

        public async Task<int> GetTotalAsync(FormStatusFilter filter, IDbController dbController)
        {
            StringBuilder sqlBuilder = new();
            sqlBuilder.AppendLine("SELECT COUNT(*) FROM form_status fs");
            sqlBuilder.AppendLine("LEFT JOIN form_status_description fsd ON (fs.status_id = fsd.status_id)");
            sqlBuilder.AppendLine("WHERE 1 = 1");
            sqlBuilder.AppendLine(GetFilterWhere(filter));

            string sql = sqlBuilder.ToString();

            int result = await dbController.GetFirstAsync<int>(sql, GetFilterParameter(filter));

            return result;
        }

        public async Task UpdateAsync(FormStatus input, IDbController dbController)
        {
            string sql = @"UPDATE form_status SET
requires_approval = @REQUIRES_APPROVAL,
is_completed = @IS_COMPLETED,
sort_order = @SORT_ORDER
WHERE status_id = @STATUS_ID";

            await dbController.QueryAsync(sql, input.GetParameters());

            await CreateOrUpdateDescriptionsAsync(input, dbController);
        }

        private async Task CreateOrUpdateDescriptionsAsync(FormStatus input, IDbController dbController)
        {
            // Delete all entries for this status id first
            string sql = "DELETE FROM form_status_description WHERE status_id = @STATUS_ID";
            await dbController.QueryAsync(sql, input.GetParameters());

            foreach (var parameters in input.GetLocalizedParameters())
            {
                sql = @"INSERT INTO form_status_description
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
)";
                await dbController.QueryAsync(sql, parameters);
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
                status.Description = descriptions.Where(x => x.StatusId == status.Id).ToList();
            }

            return statuses;
        }
    }
}
