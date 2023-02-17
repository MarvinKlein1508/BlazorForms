using DatabaseControllerProvider;

namespace FormPortal.Core.Models
{
    public class FormEntryTableElement : FormEntryElement
    {
        [CompareField("entry_table_element_id")]
        public int EntryTableElementId { get; set; }
    }
}
