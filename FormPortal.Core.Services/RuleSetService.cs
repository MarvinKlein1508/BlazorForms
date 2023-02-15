﻿using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;

namespace FormPortal.Core.Services
{
    public class RuleSetService : IModelService<Rule, int>
    {

        public async Task CreateAsync(Rule input, IDbController dbController)
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

        public Task DeleteAsync(Rule input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public Task<Rule?> GetAsync(int identifier, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Rule input, IDbController dbController)
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

        public Task UpdateAsync(Rule input, Rule oldInputToCompare, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Rule>> GetRulesForElements(List<int> elementIds, IDbController dbController)
        {
            if (!elementIds.Any())
            {
                return new();
            }

            string sql = $"SELECT * FROM form_elements_rules WHERE element_id IN ({string.Join(",", elementIds)}) ORDER BY sort_order";

            List<Rule> rules = await dbController.SelectDataAsync<Rule>(sql);

            return rules;
        }
    }
}
