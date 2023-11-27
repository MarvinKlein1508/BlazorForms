namespace BlazorForms.Core.Pdf.Models
{
    public class TableSum
    {
        public string Name { get; set; } = string.Empty;
        public string NumberFormat { get; }
        public decimal Value { get; set; }
        public TableSum(string numberFormat, decimal value)
        {
            NumberFormat = numberFormat;
            Value = value;
        }

        public TableSum(string numberFormat, decimal value, string name)
        {
            NumberFormat = numberFormat;
            Value = value;
            Name = name;
        }

    }
}
