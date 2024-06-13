using DbController;
using BlazorForms.Core.Constants;

namespace BlazorForms.Core.Models.FormElements
{
    public class FormNumberElement : FormElement
    {
        private int decimalPlaces;
        private decimal _value;

        [CompareField("decimal_places")]
        public int DecimalPlaces { get => decimalPlaces; set => decimalPlaces = value < 0 ? 0 : value; }
        [CompareField("min_value")]
        public decimal MinValue { get; set; }
        [CompareField("max_value")]
        public decimal MaxValue { get; set; }
        [CompareField("is_summable")]
        public bool IsSummable { get; set; }
        [CompareField("value_number")]
        public decimal Value
        {
            get
            {
                if (CalcRules.Count == 0)
                {
                    return _value;
                }

                decimal value = 0;

                bool isPartOfTable = TableParentElementId != 0;

                var tmp = Form?.GetCalcRuleSetElements(isPartOfTable).ToList();

                foreach (var rule in CalcRules)
                {
                    FormNumberElement? element;
                    if (GuidTableCount is not null)
                    {
                        element = Parent?.Elements.Where(x => x.ElementId == TableParentElementId)
                            .Cast<FormTableElement>()
                            .SelectMany(x => x.ElementValues)
                            .SelectMany(x => x)
                            .Where(x => x.GuidTableCount == GuidTableCount && x.GetElementType() is ElementType.Number)
                            .Cast<FormNumberElement>()
                            .FirstOrDefault(x => x.Guid == rule.GuidElement);
                        
                    }
                    else
                    {
                        element = Form?.GetCalcRuleSetElements(isPartOfTable)
                                       .FirstOrDefault(x => x.Guid == rule.GuidElement);

                    }


                    if (element is not null)
                    {
                        switch (rule.MathOperator)
                        {
                            case MathOperators.Addition:
                                value += element.Value;
                                break;
                            case MathOperators.Substraction:
                                value -= element.Value;
                                break;
                            case MathOperators.Mulitply:
                                value *= element.Value;
                                break;
                            case MathOperators.Divide:
                                if (element.Value != 0)
                                {
                                    value /= element.Value;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                return value;
            }
            set
            {
                _value = Math.Round(value, decimalPlaces);
            }
        }

        [CompareField("default_value")]
        public decimal DefaultValue { get; set; }
        public List<CalcRule> CalcRules { get; set; } = [];
        public bool IsValueCalculated => CalcRules.Count != 0;
        public override ElementType GetElementType() => ElementType.Number;
        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();

            parameters.Add("DECIMAL_PLACES", DecimalPlaces);
            parameters.Add("MIN_VALUE", MinValue);
            parameters.Add("MAX_VALUE", MaxValue);
            parameters.Add("DEFAULT_VALUE", DefaultValue);
            parameters.Add("IS_SUMMABLE", IsSummable);

            parameters["VALUE_NUMBER"] = Value;

            return parameters;
        }

        public string GetStep()
        {
            if (decimalPlaces is 0)
            {
                return "1";
            }
            else
            {
                string step = "0.";
                for (int i = 0; i < DecimalPlaces - 1; i++)
                {
                    step += "0";
                }

                step += "1";

                return step;
            }
        }

        public override string GetDefaultName() => "Number";

        public override void SetValue(FormEntryElement element)
        {
            Value = element.ValueNumber;
        }

        public override void Reset()
        {
            Value = DefaultValue;
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
