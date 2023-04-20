using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;

namespace FormPortal.Core.Services
{
    public class FormStatusService : IModelService<FormStatus, int>
    {
        public async Task CreateAsync(FormStatus input, IDbController dbController)
        {
            string sql = $@"INSERT INTO form_status
(
    requires_approval,
    is_completed
)
VALUES
(
    @REQUIRES_APPROVAL,
    @IS_COMPLETED
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

        public async Task UpdateAsync(FormStatus input, IDbController dbController)
        {
            string sql = @"UPDATE form_status SET
requires_approval = @REQUIRES_APPROVAL,
is_completed = @IS_COMPLETED,
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
    }
}
