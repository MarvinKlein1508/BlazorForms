using DatabaseControllerProvider;

namespace FormPortal.Core.Models.FormElements
{
    public class FormNumberElement : FormElement
    {
        private int decimalPlaces;
        [CompareField("decimal_places")]
        public int DecimalPlaces { get => decimalPlaces; set => decimalPlaces = value < 0 ? 0 : value; }
        [CompareField("min_value")]
        public decimal MinValue { get; set; }
        [CompareField("max_value")]
        public decimal MaxValue { get; set; }

        public decimal Value { get; set; }
        public override ElementType GetElementType() => ElementType.Number;
        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();

            parameters.Add("DECIMAL_PLACES", DecimalPlaces);
            parameters.Add("MIN_VALUE", MinValue);
            parameters.Add("MAX_VALUE", MaxValue);

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
