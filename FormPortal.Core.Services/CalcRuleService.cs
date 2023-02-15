using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;

namespace FormPortal.Core.Services
{
    public class CalcRuleService : IModelService<CalcRule, int>
    {
        public async Task CreateAsync(CalcRule input, IDbController dbController)
        {
            string sql = $@"INSERT INTO form_elements_calc_rules 
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
); {dbController.GetLastIdSql()}";

            input.CalcRuleId = await dbController.GetFirstAsync<int>(sql, input.GetParameters());
        }

        public Task DeleteAsync(CalcRule input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public Task<CalcRule?> GetAsync(int identifier, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(CalcRule input, IDbController dbController)
        {
            string sql = @"UPDATE form_elements_calc_rules SET
math_operator = @MATH_OPERATOR,
guid_element = @GUID_ELEMENT,
sort_order = @SORT_ORDER,
WHERE 
calc_rule_id = @CALC_RULE_ID";

            await dbController.QueryAsync(sql, input.GetParameters());
        }

        public Task UpdateAsync(CalcRule input, CalcRule oldInputToCompare, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CalcRule>> GetCalcRulesForElements(List<int> elementIds, IDbController dbController)
        {
            if (!elementIds.Any())
            {
                return new();
            }

            string sql = $"SELECT * FROM form_elements_calc_rules WHERE element_id IN ({string.Join(",", elementIds)}) ORDER BY sort_order";

            List<CalcRule> rules = await dbController.SelectDataAsync<CalcRule>(sql);

            return rules;
        }
    }
}
