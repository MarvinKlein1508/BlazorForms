namespace FormularPortal.Core.Models
{
    public class FormNumberElement : FormElement
    {
        private int decimalPlaces;

        public int DecimalPlaces { get => decimalPlaces; set => decimalPlaces = value < 0 ? 0 : value; }
    }
}
