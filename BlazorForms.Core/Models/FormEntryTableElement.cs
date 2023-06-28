using DbController;

namespace BlazorForms.Core.Models
{
    public class FormEntryTableElement : FormEntryElement
    {
        [CompareField("table_row_number")]
        public int TableRowNumber { get; set; }
        [CompareField("table_parent_element_id")]
        public int TableParentElementId { get; set; }
    }
}
