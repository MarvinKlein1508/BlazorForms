using BlazorForms.Core.Filters.Abstract;
using BlazorForms.Core.Models;
using DbController;
using System.Text.Json;

namespace BlazorForms.Core.Services
{
    public sealed class SavedFilterService
    {
        public async Task SaveAsync(SavedFilter input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            string sql = @$"REPLACE INTO user_filter
(
    user_id,
    filter_type,
    page,
    serialized
)
VALUES
(
    @USER_ID,
    @FILTER_TYPE,
    @PAGE,
    @SERIALIZED
)";

            await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
        }
        public async Task<T> GetAsync<T>(T defaultFilter, int user_id, string page, IDbController dbController, CancellationToken cancellationToken = default) where T : FilterBase, new()
        {

            string sql = @"SELECT * FROM user_filter WHERE filter_type = @FILTER_TYPE AND user_id = @USER_ID AND page = @PAGE";

            var result = await dbController.GetFirstAsync<SavedFilter>(sql, new
            {
                FILTER_TYPE = defaultFilter.FilterType,
                USER_ID = user_id,
                PAGE = page
            }, cancellationToken);

            if (result is not null)
            {
                var filter_deserialized = JsonSerializer.Deserialize<T>(result.Json);

                if (filter_deserialized is not null)
                {
                    return filter_deserialized;
                }
            }

            return defaultFilter;
        }



    }
}
