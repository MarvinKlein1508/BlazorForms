using DatabaseControllerProvider;
using FormularPortal.Core.Models;
using FormularPortal.Core.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Services
{
    public class RuleSetService : IModelService<RuleSet, int>
    {

        public async Task CreateAsync(RuleSet input, IDbController dbController)
        {
            string sql = @$"INSERT INTO form_elements_rules
(
element_id,
logical_operator,
element_guid,
comparison_operator,
value_boolean,
value_string,
value_number,
value_date,
sort_order
)
VALUES
(
@ELEMENT_ID,
@LOGICAL_OPERATOR,
@ELEMENT_GUID,
@COMPARISON_OPERATOR,
@VALUE_BOOLEAN,
@VALUE_STRING,
@VALUE_NUMBER,
@VALUE_DATE,
@SORT_ORDER
); {dbController.GetLastIdSql()}";

            input.RuleId = await dbController.GetFirstAsync<int>(sql, input.GetParameters());
        }

        public Task DeleteAsync(RuleSet input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public Task<RuleSet?> GetAsync(int identifier, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(RuleSet input, IDbController dbController)
        {
            string sql = @"UPDATE form_elements_rules SET
element_id = @ELEMENT_ID,
logical_operator = @LOGICAL_OPERATOR,
element_guid = @ELEMENT_GUID,
comparison_operator = @COMPARISON_OPERATOR,
value_boolean = @VALUE_BOOLEAN,
value_string = @VALUE_STRING,
value_number = @VALUE_NUMBER,
value_date = @VALUE_DATE,
sort_order = @SORT_ORDER
WHERE
rule_id = @RULE_ID";

            await dbController.QueryAsync(sql, input.GetParameters());
        }

        public Task UpdateAsync(RuleSet input, RuleSet oldInputToCompare, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public async Task<List<RuleSet>> GetRulesForElements(List<int> elementIds, IDbController dbController)
        {
            if (!elementIds.Any())
            {
                return new();
            }

            string sql = $"SELECT * FROM form_elements_rules WHERE element_id IN ({string.Join(",", elementIds)}) ORDER BY sort_order";

            List<RuleSet> rules = await dbController.SelectDataAsync<RuleSet>(sql);

            return rules;
        }
    }
}
