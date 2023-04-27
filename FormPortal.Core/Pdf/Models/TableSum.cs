namespace FormPortal.Core.Pdf.Models
{
    internal class TableSum
    {
        public string NumberFormat { get; }
        public decimal Value { get; set; }
        public TableSum(string numberFormat, decimal value)
        {
            NumberFormat = numberFormat;
            Value = value;
        }

    }
}
