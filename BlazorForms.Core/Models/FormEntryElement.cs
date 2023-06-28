using DbController;

namespace BlazorForms.Core.Models
{
    public class FormEntryElement
    {
        [CompareField("entry_id")]
        public int EntryId { get; set; }
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("element_id")]
        public int ElementId { get; set; }
        [CompareField("value_boolean")]
        public bool ValueBoolean { get; set; }
        [CompareField("value_string")]
        public string ValueString { get; set; } = string.Empty;
        [CompareField("value_number")]
        public decimal ValueNumber { get; set; }
        [CompareField("value_date")]
        public DateTime ValueDate { get; set; }
    }
}
