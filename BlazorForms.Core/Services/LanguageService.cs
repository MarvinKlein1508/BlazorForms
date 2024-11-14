using BlazorForms.Core.Models;
using DbController;

namespace BlazorForms.Core.Services
{
    public class LanguageService : IModelService<Language, int>
    {
        public Task CreateAsync(Language input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Language input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Language?> GetAsync(int identifier, IDbController dbController, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Language input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public static async Task<List<Language>> GetAllAsync(IDbController dbController)
        {
            string sql = "SELECT * FROM languages ORDER BY sort_order";

            var list = await dbController.SelectDataAsync<Language>(sql);

            return list;
        }
    }
}
