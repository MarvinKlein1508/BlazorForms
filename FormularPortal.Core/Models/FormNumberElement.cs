using DatabaseControllerProvider;
using FluentValidation;
using FormularPortal.Core.Validators.Admin;
using System.Numerics;

namespace FormularPortal.Core.Models
{
    public class FormNumberElement : FormElement
    {
        private static FormNumberElementValidator _validator = new();
        private int decimalPlaces;
        [CompareField("decimal_places")]
        public int DecimalPlaces { get => decimalPlaces; set => decimalPlaces = value < 0 ? 0 : value; }
        [CompareField("min_value")]
        public decimal MinValue { get; set; }
        [CompareField("max_value")]
        public decimal MaxValue { get; set; }

        public override ElementType GetElementType() => ElementType.Number;
        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();

            parameters.Add("DECIMAL_PLACES", DecimalPlaces);
            parameters.Add("MIN_VALUE", MinValue);
            parameters.Add("MAX_VALUE", MaxValue);

            return parameters;
        }

        public override IValidator GetValidator() => _validator;
    }
}
