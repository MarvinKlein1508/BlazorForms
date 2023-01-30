namespace FormularPortal.Core.Models
{
    public class FormElementOption
    {
        public int FormElementOptionId { get; set; }
        public int FormElementId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
