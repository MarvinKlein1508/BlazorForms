using DatabaseControllerProvider;
using FormPortal.Core.Constants;

namespace FormPortal.Core.Models.FormElements
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
        [CompareField("value_number")]
        public decimal Value
        {
            get
            {
                if (!CalcRules.Any())
                {
                    return _value;
                }

                decimal value = 0;

                foreach (var rule in CalcRules)
                {
                    var element = Form?.GetCalcRuleSetElements().FirstOrDefault(x => x.Guid == rule.GuidElement);

                    if (element is not null)
                    {
                        switch (rule.MathOperator)
                        {
                            case Constants.MathOperators.Addition:
                                value += element.Value;
                                break;
                            case Constants.MathOperators.Substraction:
                                value -= element.Value;
                                break;
                            case Constants.MathOperators.Mulitply:
                                value *= element.Value;
                                break;
                            case Constants.MathOperators.Divide:
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
            set => _value = value;
        }

        public List<CalcRule> CalcRules { get; set; } = new();

        public bool IsValueCalculated => CalcRules.Any();
        public override ElementType GetElementType() => ElementType.Number;
        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();

            parameters.Add("DECIMAL_PLACES", DecimalPlaces);
            parameters.Add("MIN_VALUE", MinValue);
            parameters.Add("MAX_VALUE", MaxValue);

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
    }
}
