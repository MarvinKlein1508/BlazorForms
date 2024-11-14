using BlazorForms.Core.Models;
using DbController;

namespace BlazorForms.Core.Services
{
    public class CalcRuleService : IModelService<CalcRule, int>
    {
        public async Task CreateAsync(CalcRule input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql =
                $"""
                INSERT INTO form_elements_number_calc_rules 
                (
                    element_id,
                    math_operator,
                    guid_element,
                    sort_order
                )
                VALUES
                (
                    @ELEMENT_ID,
                    @MATH_OPERATOR,
                    @GUID_ELEMENT,
                    @SORT_ORDER
                ); {dbController.GetLastIdSql()}
                """;

            input.CalcRuleId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);
        }

        public Task DeleteAsync(CalcRule input, IDbController dbController, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public Task<CalcRule?> GetAsync(int identifier, IDbController dbController, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public async Task UpdateAsync(CalcRule input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql =
                """
                UPDATE form_elements_number_calc_rules SET
                    math_operator = @MATH_OPERATOR,
                    guid_element = @GUID_ELEMENT,
                    sort_order = @SORT_ORDER
                WHERE 
                    calc_rule_id = @CALC_RULE_ID
                """;

            await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
        }
    }
}
