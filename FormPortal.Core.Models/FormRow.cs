using DatabaseControllerProvider;
using FormPortal.Core.Constants;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models.FormElements;
using Org.BouncyCastle.Asn1.Esf;

namespace FormPortal.Core.Models
{
    /// <summary>
    /// Represents a row within the Form.
    /// </summary>
    public class FormRow : IDbModel, IHasSortableElement, IHasRuleSet
    {
        [CompareField("row_id")]
        public int RowId { get; set; }
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("is_active")]
        public bool IsActive { get; set; } = true;
        [CompareField("rule_type")]
        public RuleType RuleType { get; set; }
        [CompareField("sort_order")]
        public int SortOrder { get; set; }

        public int Id => RowId;
        /// <summary>
        /// Gets or sets all columns for this row.
        /// </summary>
        public List<FormColumn> Columns { get; set; } = new();
        public List<Rule> Rules { get; set; } = new();
        public Form? Form { get; set; }

        /// <summary>
        /// Creates an empty row.
        /// </summary>
        public FormRow()
        {

        }
        /// <summary>
        /// Creates a new row with a specified amount of columns.
        /// </summary>
        /// <param name="columns"></param>
        public FormRow(Form form, int columns)
        {
            for (int i = 0; i < columns; i++)
            {
                Columns.Add(new FormColumn(form));
            }
        }

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "ROW_ID", RowId },
                { "FORM_ID", FormId },
                { "IS_ACTIVE", IsActive },
                { "RULE_TYPE", RuleType.ToString() },
                { "SORT_ORDER", SortOrder }
            };
        }



        public IEnumerable<FormElement> GetElements()
        {
            foreach (var column in Columns)
            {
                foreach (var element in column.Elements)
                {
                    yield return element;
                }
            }
        }

        public string GetColumnClass()
        {
            if (Columns.Count is 1)
            {
                return "col-12";
            }

            if (Columns.Count is 2)
            {
                return "col-md-6";
            }

            if (Columns.Count is 3)
            {
                return "col-md-4";
            }

            if (Columns.Count is 4)
            {
                return "col-md-3";
            }

            return "col";
        }

        public bool IsVisible()
        {
            if (!IsActive)
            {
                return false;
            }

            if (RuleType is not RuleType.Visible)
            {
                return true;
            }

            if (!Rules.Any())
            {
                return true;
            }

            // Check if all rules are true
            return ValidateRules();
        }

        private bool ValidateRules()
        {
            if (!Rules.Any())
            {
                return true;
            }


            List<(LogicalOperator logicOperator, bool result)> checkedRules = new();

            foreach (var rule in Rules)
            {
                var element = rule.Element;
                if (element is FormNumberElement numberElement)
                {
                    bool result = rule.ComparisonOperator switch
                    {
                        ComparisonOperator.Equals => numberElement.Value == rule.ValueNumber,
                        ComparisonOperator.NotEquals => numberElement.Value != rule.ValueNumber,
                        ComparisonOperator.GreaterThan => numberElement.Value > rule.ValueNumber,
                        ComparisonOperator.LessThan => numberElement.Value < rule.ValueNumber,
                        ComparisonOperator.GreaterThanOrEqual => numberElement.Value >= rule.ValueNumber,
                        ComparisonOperator.LessThanOrEqual => numberElement.Value <= rule.ValueNumber,
                        _ => false,
                    };

                    checkedRules.Add((rule.LogicalOperator, result));
                }
                else if (element is FormElementWithOptions optionsElement)
                {
                    bool result = rule.ComparisonOperator switch
                    {
                        ComparisonOperator.Equals => optionsElement.Value == rule.ValueString,
                        ComparisonOperator.NotEquals => optionsElement.Value != rule.ValueString,
                        _ => false,
                    };

                    checkedRules.Add((rule.LogicalOperator, result));
                }
                else if (element is FormDateElement dateElement)
                {
                    bool result = rule.ComparisonOperator switch
                    {
                        ComparisonOperator.Equals => dateElement.Value.Date == rule.ValueDate.Date,
                        ComparisonOperator.NotEquals => dateElement.Value.Date != rule.ValueDate.Date,
                        ComparisonOperator.GreaterThan => dateElement.Value.Date > rule.ValueDate.Date,
                        ComparisonOperator.LessThan => dateElement.Value.Date < rule.ValueDate.Date,
                        ComparisonOperator.GreaterThanOrEqual => dateElement.Value.Date >= rule.ValueDate.Date,
                        ComparisonOperator.LessThanOrEqual => dateElement.Value.Date <= rule.ValueDate.Date,
                        _ => false,
                    };

                    checkedRules.Add((rule.LogicalOperator, result));
                }
                else if (element is FormCheckboxElement checkboxElement)
                {
                    bool result = rule.ComparisonOperator switch
                    {
                        ComparisonOperator.Equals => checkboxElement.Value == rule.ValueBoolean,
                        ComparisonOperator.NotEquals => checkboxElement.Value != rule.ValueBoolean,
                        _ => false,
                    };

                    checkedRules.Add((rule.LogicalOperator, result));
                }
            }

            // When someone has manipulated the values within the database then me might not have any results at all.
            if (!checkedRules.Any())
            {
                return false;
            }

            bool returnResult = checkedRules.First().result;

            for (int i = 1; i < checkedRules.Count; i++)
            {
                (LogicalOperator logicOperator, bool result) tmp = checkedRules[i];

                returnResult = tmp.logicOperator switch
                {
                    LogicalOperator.And => returnResult && tmp.result,
                    LogicalOperator.Or => returnResult || tmp.result,
                    _ => false,
                };
            }


            return returnResult;
        }


    }
}
